import { useEffect, useState } from "react";
import { getPreviousRidesForDriver } from "../../../services/ride.service";
import { DecodeToken } from "../../../services/token-decoder";
import styles from "../previous-rides/previous-rides.module.css";
import { RideModel } from "../../../shared/models/ride";

export function PreviousRides() {
  const [rides, setRides] = useState<{ data: RideModel[] }>({ data: [] });
  const getRides = async () => {
    try{
      var response = await getPreviousRidesForDriver(DecodeToken().user_id);
      setRides({ data: response.rides });
    }
    catch{
      console.log("Error getting previous rides");
    }
  };

  useEffect(() => {
    getRides();
  }, []);

  return (
    <div className={styles.wrapper}>
      <div className={styles.page}>
        <h1 className="mb-4">Your previous rides</h1>

        {rides.data.length === 0 ? (
          <div>
            <p>You have no previous rides...</p>
          </div>
        ) : (
          <table className={`table table-striped ${styles.table}`}>
            <thead>
              <tr>
                <th>Starting Point</th>
                <th>Ending Point</th>
                <th>Price - $</th>
                <th>Travel time - min</th>
              </tr>
            </thead>
            <tbody>
              {rides.data.map((row, i) => (
                <tr key={i}>
                  <td>{row.startingPoint}</td>
                  <td>{row.endingPoint}</td>
                  <td>{row.price}</td>
                  <td>{Math.floor(row.arrivalTimeInSeconds / 60)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}
