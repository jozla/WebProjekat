import { useEffect, useState } from "react";
import { confirmRide, getConfirmedRide, getNewRides } from "../../../services/ride.service";
import styles from "../driver-dashboard/driver-dashboard.module.css";
import { useNavigate } from "react-router-dom";
import { RideModel } from "../../../shared/models/ride";
import { DecodeToken } from "../../../services/token-decoder";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { Header } from "../../../shared/header/header";

export function DriverDashboard() {
  const [rides, setRides] = useState<{ data: RideModel[] }>({ data: [] });
  const [ride, setRide] = useState<RideModel | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    getRides();

    const connection = new HubConnectionBuilder().withUrl("http://localhost:8389/chatHub", { withCredentials: false }).build();

    connection
      .start()
      .then(() => {
        console.log("Connected to SignalR hub");
        connection.invoke("JoinGroup", "Driver");
      })
      .catch((err) => console.error("Error connecting to hub:", err));

    connection.on("SendMessage", (message) => {
      getRides();
    });
  }, []);

  const getRides = async () => {
    try {
      var response = await getNewRides();
      setRides({ data: response.rides });
    } catch {
      console.log("Error getting new rides");
    }
  };

  const generateRandomTime = () => {
    var randomArrivalTime = Math.floor(Math.random() * 3600);
    return Math.max(randomArrivalTime, 300);
  };

  const handleConfirmRide = async (id: string) => {
    try {
      var decodedToken = DecodeToken();
      var data = {
        rideId: id,
        driverId: decodedToken.user_id,
        arrivalTimeInSeconds: generateRandomTime(),
      };
      await confirmRide(data);
      getRide();
    } catch (error) {
      console.error("Failed to confirm ride:", error);
      alert("Failed to confirm ride");
    }
  };

  const getRide = async () => {
    try {
      var response = await getConfirmedRide(DecodeToken().user_id);
      setRide(response.ride as RideModel);
    } catch {
      console.log("Error getting new rides");
    }
  };

  useEffect(() => {
    if (ride) {
      console.log(ride);
      navigate("/timer", {
        state: {
          initialMinute: 0,
          initialSeconds: 5,
          arrivalMinute: 0,
          arrivalSeconds: 5,
          rideId: ride.id,
          driverId: ride.driverId,
        },
      });
    }
  }, [ride, navigate]);

  const handleSeePreviousRides = () => {
    navigate("/driver/previous-rides");
  };

  return (
    <>
      <Header></Header>
      {DecodeToken().verification === "Processing" || DecodeToken().verification === "Unverified" ? (
        <div>
          <h2>You are account is not verified or is blocked...</h2>
        </div>
      ) : (
        <div className={styles.wrapper}>
          <div className={styles.page}>
            <div className={`mb-3 ${styles.previousButtonAndHeader}`}>
              <h1 className="mb-0">New available rides</h1>
              <button onClick={handleSeePreviousRides} className="btn btn-dark">
                Your previous rides
              </button>
            </div>
            {rides.data.length === 0 ? (
              <div>
                <p>There are no new rides...</p>
              </div>
            ) : (
              <table className={`table table-striped ${styles.table}`}>
                <thead>
                  <tr>
                    <th>Starting Point</th>
                    <th>Ending Point</th>
                    <th>Time to Reach - min</th>
                  </tr>
                </thead>
                <tbody>
                  {rides.data.map((row, i) => (
                    <tr key={i}>
                      <td>{row.startingPoint}</td>
                      <td>{row.endingPoint}</td>
                      <td>{Math.floor(row.driverTimeInSeconds / 60)}</td>
                      <td>
                        <button onClick={() => handleConfirmRide(row.id)} className="btn btn-success">
                          Confirm
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      )}
    </>
  );
}
