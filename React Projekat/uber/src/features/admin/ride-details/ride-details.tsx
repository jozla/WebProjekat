import { useEffect, useState } from "react";
import { getUserById } from "../../../services/user.service";
import { UserModel } from "../../../shared/models/user";

export default function RideDetails({ passengerId, driverId }) {
    const [passenger, setPassenger] = useState<UserModel|null>(null);
    const [driver, setDriver] = useState<UserModel|null>(null);
  
    useEffect(() => {
      if (passengerId) {
        getPassengerData();
      }
      if (driverId) {
        getDriverData();
      }
    }, [passengerId, driverId]);
  
    const getPassengerData = async () => {
        var response = await getUserById(passengerId);
        setPassenger(response.user);
    };

    const getDriverData = async () => {
        var response = await getUserById(driverId);
        setDriver(response.user);
    };

    return (
      <div>
        {passenger && (
          <div>
            <h3>Passenger Details</h3>
            <p>Name: {passenger.name}</p>
            <p>Last name: {passenger.lastName}</p>
            <p>Email: {passenger.email}</p>
          </div>
        )}
        {driver && (
          <div>
            <h3>Driver Details</h3>
            <p>Name: {driver.name}</p>
            <p>Last naeme: {driver.lastName}</p>
            <p>Email: {driver.email}</p>
          </div>
        )}
      </div>
    );
  }