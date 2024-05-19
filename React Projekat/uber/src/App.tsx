import './App.css';
import LogIn from './features/login/login-page';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Register from './features/register/register-page';
import { AddRide } from './features/user/add-ride/add-ride';
import { UserDashboard } from './features/user/user-dashboard/user-dashboard';
import { DriverDashboard } from './features/driver/driver-dashboard/driver-dashborad';
import { PreviousRides } from './features/driver/previous-rides/previous-rides';

function App() {
  return (
    <BrowserRouter>
    <Routes>
      <Route path="/" element={<LogIn />}/>
      <Route path="register" element={<Register />} />
      <Route path="user/dashboard" element={<UserDashboard />} />
      <Route path="user/add-ride" element={<AddRide />} />
      <Route path="driver/dashboard" element={<DriverDashboard />} />
      <Route path="driver/previous-rides" element={<PreviousRides />} />
    </Routes>
  </BrowserRouter>
  );
}
export default App;
