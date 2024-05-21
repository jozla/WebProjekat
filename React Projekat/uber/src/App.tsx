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
import AdminDashboard from './features/admin/admin-dashboard/admin-dashboard';
import { Rides } from './features/admin/rides/rides';
import RequireRole from './shared/guard/require-role';
import RequireAuth from './shared/guard/require-auth';

function App() {
  return (
    <BrowserRouter>
    <Routes>
      <Route path="/" element={<LogIn />}/>
      <Route path="register" element={<Register />} />
      <Route path="update-profile" element={<RequireAuth><UpdateProfile /></RequireAuth>} />
      <Route path="timer" element={<RequireAuth><Timer /></RequireAuth>} />
      <Route path="user/dashboard" element={<RequireRole role="User"><UserDashboard /></RequireRole>} />
      <Route path="user/add-ride" element={<RequireRole role="User"><AddRide /></RequireRole>} />
      <Route path="user/waiting-page" element={<RequireRole role="User"><WaitingPage /></RequireRole>} />
      <Route path="user/rate-user" element={<RequireRole role="User"><RateUser /></RequireRole>} />
      <Route path="driver/dashboard" element={<RequireRole role="Driver"><DriverDashboard /></RequireRole>} />
      <Route path="driver/previous-rides" element={<RequireRole role="Driver"><PreviousRides /></RequireRole>} />
      <Route path="admin/dashboard" element={<RequireRole role="Admin"><AdminDashboard /></RequireRole>} />
      <Route path="admin/rides" element={<RequireRole role="Admin"><Rides /></RequireRole>} />
    </Routes>
  </BrowserRouter>
  );
}
export default App;
