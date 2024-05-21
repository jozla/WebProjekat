import { useLocation, useNavigate } from "react-router-dom";
import styles from "../rate/rate.module.css";
import { Rating } from "react-simple-star-rating";
import { addRating } from "../../../services/rating.service";

export default function RateUser() {
  const state = useLocation();
  const { driverId } = state.state;
  const navigate = useNavigate();

  const handleRating = async (rate:number) => {
    try{
      await addRating({userId: driverId, rating:rate});
      navigate('/user/dashboard');
    }
    catch{
      console.log('Error adding rating');
    }
  };

  return (
    <div className={`${styles.page}`}>
      <h2>How was your experience on this ride?</h2>
      <Rating onClick={handleRating} />
    </div>
  );
}
