import { useEffect, useState } from "react";
import { Header } from "../../../shared/header/header";
import styles from "../rides/rides.module.css";
import { useNavigate } from "react-router-dom";
import { RideModel } from "../../../shared/models/ride";
import { getAllRides } from "../../../services/ride.service";
import RideDetails from "../ride-details/ride-details";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from "@mui/material";

export function Rides() {
  const [rides, setRides] = useState<{ data: RideModel[] }>({ data: [] });
  const [selectedRide, setSelectedRide] = useState({ passengerId: "", driverId: "" });
  const [open, setOpen] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    getRides();
  }, []);

  const getRides = async () => {
    try {
      var response = await getAllRides();
      setRides({ data: response.rides });
    } catch {
      console.log("Error getting new rides");
    }
  };

  const handleShowModal = (passengerId, driverId) => {
    setSelectedRide({ passengerId, driverId });
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };

  return (
    <>
      <Header></Header>
      <div className={styles.wrapper}>
        <div className={styles.page}>
          <h1 className="mb-4">All rides</h1>
          {rides.data.length === 0 ? (
            <div>
              <p>There are no rides...</p>
            </div>
          ) : (
            <table className={`table table-striped ${styles.table}`}>
              <thead>
                <tr>
                  <th>Starting Point</th>
                  <th>Ending Point</th>
                  <th>Ride time - min</th>
                  <th>Price - $</th>
                  <th>Status</th>
                  <th>DriverId</th>
                  <th>PassengerId</th>
                </tr>
              </thead>
              <tbody>
                {rides.data.map((row, i) => (
                  <tr key={i}>
                    <td>{row.startingPoint}</td>
                    <td>{row.endingPoint}</td>
                    <td>{Math.floor((row.driverTimeInSeconds + row.arrivalTimeInSeconds) / 60)}</td>
                    <td>{row.price}</td>
                    <td>
                      {row.status === 0 && <span>Pending</span>}
                      {row.status === 1 && <span>Confirmed</span>}
                      {row.status === 2 && <span>Finished</span>}
                    </td>
                    <td>{row.driverId === "00000000-0000-0000-0000-000000000000" ? "N/A" : row.driverId}</td>
                    <td>{row.passengerId}</td>
                    <td>
                      <button
                        className="btn btn-dark"
                        onClick={() => handleShowModal(row.passengerId, row.driverId)}
                      >
                        Check details
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>

      <div>
      <Dialog open={open} onClose={handleClose}>
        <DialogContent>
          <RideDetails passengerId={selectedRide.passengerId} driverId={selectedRide.driverId} />
        </DialogContent>
        <DialogActions>
          <button className="btn btn-dark" onClick={handleClose}>Close</button>
        </DialogActions>
      </Dialog>
    </div>
    </>
  );
}
