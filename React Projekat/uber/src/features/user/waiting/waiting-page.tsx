import { HubConnectionBuilder } from "@microsoft/signalr";
import { useEffect, useState } from "react";
import styles from "../waiting/waiting-page.module.css";
import { useLocation, useNavigate } from "react-router-dom";
import { getConfirmedRide } from "../../../services/ride.service";
import { RideModel } from "../../../shared/models/ride";
import { DecodeToken } from "../../../services/token-decoder";

export default function WaitingPage() {
  const navigate = useNavigate();
  const state = useLocation();
  const [ride, setRide] = useState<RideModel | null>(null);
  const [passengerId, setPassengerId] = useState<string|null>(null);

  useEffect(() => {
    const {passengerId} = state.state;
    setPassengerId(passengerId);
    
    const connection = new HubConnectionBuilder().withUrl(process.env.REACT_APP_CONN_HUB_URL!, { withCredentials: false }).build();
    connection
      .start()
      .then(() => {
        console.log("Connected to SignalR hub");
        connection.invoke("JoinGroup", passengerId);
      })
      .catch((err) => console.error("Error connecting to hub:", err));

    connection.on("SendMessage", (message) => {
      getRide();
    });
  }, []);

  useEffect(() => {
    if (ride) {
      navigate("/timer", {
        state: {
          initialMinute: 0,
          // initialMinute: Math.floor((ride.driverTimeInSeconds) / 60),
          initialSeconds: 15,
          // initialSeconds: ride.driverTimeInSeconds % 60,
          arrivalMinute: 0,
          // arrivalMinute: Math.floor((ride.arrivalTimeInSeconds) / 60)
          arrivalSeconds: 5,
          // arrivalSeconds: ride.arrivalTimeInSeconds % 60
          rideId: ride.id,
          driverId: ride.driverId,
          passengerId : passengerId,
          isPassenger : true
        },
      });
    }
  }, [ride, navigate]);

  const getRide = async () => {
    try {
      var response = await getConfirmedRide(DecodeToken()!.user_id);
      setRide(response.ride as RideModel);
    } catch {
      console.log("Error getting new rides");
    }
  };

  return (
    <div className={`d-flex justify-content-center ${styles.page}`}>
      <h2> Waiting for ride to be confirmed...</h2>
    </div>
  );
}
