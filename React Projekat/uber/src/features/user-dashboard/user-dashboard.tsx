import { useEffect, useState } from "react";
import { getUserRides } from "../../services/ride.service";
import { DecodeToken } from "../../services/token-decoder";
import styles from "../user-dashboard/user-dashboard.module.css";
import { useNavigate } from "react-router-dom";

export function UserDashboard() {
  interface RideModel {
    id: string;
    startingPoint: string;
    endingPoint: string;
    price: number;
    driverTimeInSeconds: number;
    arrivalTimeInSeconds: number;
    driverId: string;
    passengerId: string;
    status: number;
  }
  const [rides, setRides] = useState<{ data: RideModel[] }>({ data: [] });
  const getRides = async () => {
    var response = await getUserRides(DecodeToken().user_id);
    setRides({ data: response.rides });
  };

  useEffect(() => {
    getRides();
  }, []);
  
  const navigate = useNavigate();

  const handleAddRide = () => {
    navigate('/user/add-ride')
  };

  return (
    <div className={styles.wrapper}>
      <div className={styles.page}>
        <div className={`mb-3 ${styles.addButtonAndHeader}`}>
            <h1 className="mb-0">Your rides</h1>
            <button onClick={handleAddRide} className="btn btn-primary">
            + Add New Ride
            </button>
        </div>
        {rides.data.length === 0 ? (
          <div>
            <p>You have no rides, you can add new one by pressing the AddRide button...</p>
          </div>
        ) : (
          <table className={`table table-striped ${styles.table}`}>
            <thead>
              <tr>
                <th>Starting Point</th>
                <th>Ending Point</th>
                <th>Price</th>
              </tr>
            </thead>
            <tbody>
              {rides.data.map((row, i) => (
                <tr key={i}>
                  <td>{row.startingPoint}</td>
                  <td>{row.endingPoint}</td>
                  <td>{row.price}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}
