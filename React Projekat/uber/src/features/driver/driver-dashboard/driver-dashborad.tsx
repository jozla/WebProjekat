import { useEffect, useState } from "react";
import { confirmRide, getNewRides } from "../../../services/ride.service";
import styles from "../driver-dashboard/driver-dashboard.module.css";
import { useNavigate } from "react-router-dom";
import { RideModel } from "../../../shared/models/ride";
import { DecodeToken } from "../../../services/token-decoder";

export function DriverDashboard() {
    const [rides, setRides] = useState<{ data: RideModel[] }>({ data: [] });
    const navigate = useNavigate();
  
  useEffect(() => {
    getRides();
  }, []);
  
  const getRides = async () => {
    var response = await getNewRides();
    setRides({ data: response.rides });
  };
  
  const generateRandomTime = () => {
    var randomArrivalTime = Math.floor(Math.random() * 3600);
    return Math.max(randomArrivalTime, 300);
  }

  const handleConfirmRide = async (id:string) => {
    try {
      var decodedToken = DecodeToken();
      var data = {
        rideId : id,
        driverId: decodedToken.user_id,
        arrivalTimeInSeconds: generateRandomTime()
      }
      await confirmRide(data);
      alert('Ride confirmed successfully');
    } catch (error) {
      console.error('Failed to confirm ride:', error);
      alert('Failed to confirm ride');
    }
  };
  
  const handleSeePreviousRides = () => {
    navigate('/driver/previous-rides');
  };

  return (
    <div className={styles.wrapper}>
      <div className={styles.page}>
      <div className={`mb-3 ${styles.previousButtonAndHeader}`}>
            <h1 className="mb-0">New available rides</h1>
            <button onClick={handleSeePreviousRides} className="btn btn-primary">
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
                  <button 
                    onClick={() => handleConfirmRide(row.id)} 
                    className="btn btn-success"
                  >
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
  );
}
