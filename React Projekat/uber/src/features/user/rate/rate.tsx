import { useLocation } from "react-router-dom";
import styles from "../rate/rate.module.css";
export default function RateUser() {
    const state = useLocation();
    const { driverId } = state.state;
    return(
        <div className={`${styles.page}`}>
            <h2>How was your experience on this ride?</h2>
        </div>
    )
}