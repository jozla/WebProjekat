import './App.css';
import LogIn from './features/login/login-page';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Register from './features/register/register-page';
import { AddRide } from './features/user/add-ride/add-ride';
import { UserDashboard } from './features/user/user-dashboard/user-dashboard';
import { DriverDashboard } from './features/driver/driver-dashboard/driver-dashboard';
import { PreviousRides } from './features/driver/previous-rides/previous-rides';
import WaitingPage from './features/user/waiting/waiting-page';
import Timer from './shared/timer/timer';
import RateUser from './features/user/rate/rate';
import UpdateProfile from './features/update-profile/update-profile';

function App() {
  return (
    <BrowserRouter>
    <Routes>
      <Route path="/" element={<LogIn />}/>
      <Route path="register" element={<Register />} />
      <Route path="update-profile" element={<UpdateProfile />} />
      <Route path="user/dashboard" element={<UserDashboard />} />
      <Route path="user/add-ride" element={<AddRide />} />
      <Route path="user/waiting-page" element={<WaitingPage />} />
      <Route path="user/rate-user" element={<RateUser />} />
      <Route path="driver/dashboard" element={<DriverDashboard />} />
      <Route path="driver/previous-rides" element={<PreviousRides />} />
      <Route path="timer" element={<Timer />} />
    </Routes>
  </BrowserRouter>
  );
}
export default App;
