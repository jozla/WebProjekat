import './App.css';
import LogIn from './features/login/login-page';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Register from './features/register/register-page';
import { AddRide } from './features/add-ride/add-ride';
import { UserDashboard } from './features/user-dashboard/user-dashboard';

function App() {
  return (
    <BrowserRouter>
    <Routes>
      <Route path="/" element={<LogIn />}/>
      <Route path="register" element={<Register />} />
      <Route path="user/dashboard" element={<UserDashboard />} />
      <Route path="user/add-ride" element={<AddRide />} />
    </Routes>
  </BrowserRouter>
  );
}
export default App;
