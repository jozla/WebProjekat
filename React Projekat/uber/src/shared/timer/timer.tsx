import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import styles from '../timer/timer.module.css';
import { DecodeToken } from "../../services/token-decoder";
import { finishRide } from "../../services/ride.service";

export default function Timer() {
  const state = useLocation();
  const { initialMinute , initialSeconds, arrivalMinute, arrivalSeconds, rideId, driverId } = state.state;
  const [minutes, setMinutes] = useState(initialMinute);
  const [seconds, setSeconds] = useState(initialSeconds);
  const [driverArrival, setDriverArrival] = useState(true);


  const navigate = useNavigate();

  useEffect(() => {
    let myInterval = setInterval(() => {
        if (seconds > 0) {
          setSeconds(seconds - 1);
        }
        if (seconds === 0) {
          if (minutes === 0) {
            if (driverArrival) {
              setMinutes(arrivalMinute);
              setSeconds(arrivalSeconds);
              setDriverArrival(false);
            } else {
              if(DecodeToken().user_role == 'User' ){
                navigate("/user/rate-user", {state: {driverId: driverId}})
              }
              else {
                handleFinishRide();
                navigate("/driver/dashboard");
              }
            }
          } else {
            setMinutes(minutes - 1);
            setSeconds(59);
          }
        }
      }, 1000);
      return () => {
        clearInterval(myInterval);
      };
  });

  const handleFinishRide = async() => {
    let data = {
      rideId : rideId
    }
    await finishRide(data);
  }

  return (
      <div className={styles.page}>
        <h2>{driverArrival ? "Time until the ride starts:" : "Time until you arrive at the destination:"}</h2>
        <h1>
          {" "}
          {minutes}:{seconds < 10 ? `0${seconds}` : seconds}
        </h1>
      </div>
  );
}
