import { useNavigate } from "react-router-dom";
import { Header } from "../../../shared/header/header";
import styles from "../admin-dashboard/admin-dashboard.module.css";
import { useEffect, useState } from "react";
import { UserModel } from "../../../shared/models/user";
import { blockUser, getAllDrivers, verifyUser } from "../../../services/user.service";
import { getRating } from "../../../services/rating.service";

export default function AdminDashboard() {
  const navigate = useNavigate();
  const [users, setUsers] = useState<{ data: UserModel[] }>({ data: [] });

  useEffect(() => {
    getUsers();
  }, []);

  const handleAllRides = () => {
    navigate("/admin/rides");
  };

  const getUsers = async () => {
    try {
      var response = await getAllDrivers();
      setUsers({ data: response.drivers });
    } catch {
      console.log("Error getting new rides");
    }
  };

  const handleVerification = async (userId: string) => {
    try {
      await verifyUser({ id: userId });
      getUsers();
    } catch {
      console.log("Error verifying user");
    }
  };

  const handleBlock = async (userId: string) => {
    try {
      await blockUser({ id: userId });
      getUsers();
    } catch {
      console.log("Error blocking user");
    }
  };

  const seeRating = async(userId: string) => {
    try{
      var response = await getRating(userId);
      alert('This user\'s rating is: '+ response.rating.toFixed(1))
    }
    catch{
      alert("There is no rating for this user.");
    }
  }

  return (
    <>
      <Header></Header>
      <div className={styles.wrapper}>
        <div className={styles.page}>
          <div className={`mb-3 ${styles.ridesButtonAndHeader}`}>
            <h1 className="mb-0">List of drivers</h1>
            <button onClick={handleAllRides} className="btn btn-dark">
              See all rides
            </button>
          </div>
          {users.data.length === 0 ? (
            <div>
              <p>There are no users...</p>
            </div>
          ) : (
            <table className={`table table-striped ${styles.table}`}>
              <thead>
                <tr>
                  <th>Username</th>
                  <th>Name</th>
                  <th>LastName</th>
                  <th>Email</th>
                  <th>Birthday</th>
                  <th>Address</th>
                  <th>State</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {users.data.map((row, i) => (
                  <tr key={i}>
                    <td>{row.userName}</td>
                    <td>{row.name}</td>
                    <td>{row.lastName}</td>
                    <td>{row.email}</td>
                    <td>{row.birthday}</td>
                    <td>{row.address}</td>
                    <td>
                      {row.verificationState === 0 && <span>Verified</span>}
                      {row.verificationState === 1 && <span>Blocked</span>}
                      {row.verificationState === 2 && <span>Pending</span>}
                    </td>
                    <td>
                      {row.verificationState === 0 && (
                        <button className="btn btn-danger" onClick={() => handleBlock(row.id)}>
                          Block
                        </button>
                      )}
                      {row.verificationState === 1 && (
                        <button className="btn btn-success" onClick={() => handleVerification(row.id)}>
                          Unblock
                        </button>
                      )}
                      {row.verificationState === 2 && (
                        <button className="btn btn-success" onClick={() => handleVerification(row.id)}>
                          Verify
                        </button>
                      )}
                    </td>
                    <td>
                      <button className="btn btn-dark" onClick={() => seeRating(row.id)}>
                        Rating
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </>
  );
}
