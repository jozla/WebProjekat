import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

export default function Timer() {
  const state = useLocation();
  const { initialMinute , initialSeconds, arrivalMinute, arrivalSeconds } = state.state;
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
              navigate("/user/dashboard");
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

  return (
    <div>
      <h1>
        {" "}
        {minutes}:{seconds < 10 ? `0${seconds}` : seconds}
      </h1>
    </div>
  );
}
