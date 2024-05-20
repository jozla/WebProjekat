import { useLocation } from "react-router-dom";

export default function RateUser() {
    const state = useLocation();
    const { driverId } = state.state;
    return(
        <div>
            Rating user...
        </div>
    )
}