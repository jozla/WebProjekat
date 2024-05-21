import { HubConnectionBuilder } from "@microsoft/signalr";
import { useEffect, useState } from "react";
import styles from "../waiting/waiting-page.module.css";
import { useNavigate } from "react-router-dom";
import { getConfirmedRide } from "../../../services/ride.service";
import { RideModel } from "../../../shared/models/ride";
import { DecodeToken } from "../../../services/token-decoder";

export default function WaitingPage() {
  const navigate = useNavigate();
  const [ride, setRide] = useState<RideModel | null>(null);

  useEffect(() => {
    const connection = new HubConnectionBuilder().withUrl("http://localhost:8389/chatHub", { withCredentials: false }).build();
    connection
      .start()
      .then(() => {
        console.log("Connected to SignalR hub");
        connection.invoke("JoinGroup", "User");
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
          initialSeconds: 5,
          arrivalMinute: 0,
          arrivalSeconds: 5,
          rideId: ride.id,
          driverId: ride.driverId
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
