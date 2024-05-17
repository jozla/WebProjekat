import './App.css';
import LogIn from './features/login/login-page';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Register from './features/register/register-page';

function App() {
  return (
    <BrowserRouter>
    <Routes>
      <Route path="/" element={<LogIn />}/>
      <Route path="register" element={<Register />} />
    </Routes>
  </BrowserRouter>
  );
}
export default App;
