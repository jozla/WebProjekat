import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import styles from '../timer/timer.module.css';
import { DecodeToken } from "../../services/token-decoder";
import { finishRide } from "../../services/ride.service";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { sendMessage } from "../../services/user.service";

export default function Timer() {
  const state = useLocation();
  const [initialMinute, setInitialMinute] = useState<number|null>(null);
  const [initialSeconds, setInitialSeconds] = useState<number|null>(null);
  const [arrivalMinute, setArrivalMinute] = useState(null);
  const [arrivalSeconds, setArrivalSeconds] = useState(null);
  const [rideId, setRideId] = useState(null);
  const [driverId, setDriverId] = useState(null);
  const [passengerId, setPassengerId] = useState(null);
  const [minutes, setMinutes] = useState(initialMinute);
  const [seconds, setSeconds] = useState(initialSeconds);
  const [driverArrival, setDriverArrival] = useState(true);
  const [isPassenger, setIsPassenger] = useState(false);
  const [message, setMessage] = useState("");
  const [messages, setMessages] = useState<string[]>([]);


  const navigate = useNavigate();
  const anyValueIsNull = [initialMinute, initialSeconds, arrivalMinute, arrivalSeconds, rideId, driverId].some(value => value === null);

  useEffect(() => {
    if (state.state) {
      const { initialMinute, initialSeconds, arrivalMinute, arrivalSeconds, rideId, driverId, passengerId, isPassenger } = state.state;
      setInitialMinute(initialMinute);
      setInitialSeconds(initialSeconds);
      setArrivalMinute(arrivalMinute);
      setArrivalSeconds(arrivalSeconds);
      setRideId(rideId);
      setDriverId(driverId);
      setMinutes(initialMinute);
      setSeconds(initialSeconds);
      setPassengerId(passengerId);
      setIsPassenger(isPassenger);
    }
  }, [state.state]);

  useEffect(() => {
    if (anyValueIsNull) return;
    let myInterval = setInterval(() => {
        if (seconds! > 0) {
          setSeconds(seconds! - 1);
        }
        if (seconds === 0) {
          if (minutes === 0) {
            if (driverArrival) {
              setMinutes(arrivalMinute);
              setSeconds(arrivalSeconds);
              setDriverArrival(false);
            } else {
              if(DecodeToken()!.user_role === 'User'){
                navigate("/user/rate-user", {state: {driverId: driverId}})
              }
              else {
                handleFinishRide();
                navigate("/driver/dashboard");
              }
            }
          } else {
            setMinutes(minutes! - 1);
            setSeconds(59);
          }
        }
      }, 1000);
      return () => {
        clearInterval(myInterval);
      };
  });

  useEffect(() => {
    if(isPassenger) {
      connect(passengerId);
    }
    else{
      connect(driverId);
    }
  },[isPassenger,passengerId,driverId]);

  const connect = (group:string|null) => {
    if(group){
      const connection = new HubConnectionBuilder().withUrl(process.env.REACT_APP_CONN_HUB_URL!, { withCredentials: false }).build();
  
      connection
        .start()
        .then(() => {
          console.log("Connected to SignalR hub");
          connection.invoke("JoinGroup", group);
        })
        .catch((err) => console.error("Error connecting to hub:", err));
  
      connection.on("SendMessage", (message) => {
        console.log(message);
        setMessages(prevMessages => [...prevMessages, `User:   ${message}`]);
      });
    }
  }

  const handleFinishRide = async() => {
    let data = {
      rideId : rideId
    }
    await finishRide(data);
  }

  const handleSendMessage = async() => {
    if(isPassenger){
      setMessages(prevMessages => [...prevMessages, `Me:   ${message}`]);
      await sendMessage({userId: driverId, message:message})
    }
    else{
      setMessages(prevMessages => [...prevMessages, `Me:   ${message}`]);
      await sendMessage({userId: passengerId, message:message})
    }
  }

  if (anyValueIsNull) {
    return (
      <div className={styles.page}>
        <h2>You can't access the timer now...</h2>
      </div>
    );
  }

  return (
      <div className={styles.page}>
        <h2>{driverArrival ? "Time until the ride starts:" : "Time until you arrive at the destination:"}</h2>
        <h1>
          {" "}
          {minutes}:{seconds! < 10 ? `0${seconds}` : seconds}
        </h1>

        <h1 className="mt-4">Live chat</h1>
        <div className={styles.chatMessages}>
          {messages.map((msg, index) => (
            <div key={index}>{msg}</div>
          ))}
        </div>
        <input
          className={`form-control ${styles.field} mt-3`}
          type="text"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          placeholder="Type your message here"
        />
        <button className={`btn btn-dark mt-2 ${styles.field}`} onClick={handleSendMessage}>
          Send
        </button>
      </div>
  );
}
