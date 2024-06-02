import { useNavigate } from "react-router-dom";
import { DecodeToken } from "../../services/token-decoder";
import styles from '../header/header.module.css';
import { googleLogout } from "@react-oauth/google";

export function Header(){
    const navigate = useNavigate();
    
    const handleUpdateProfile = () => {
        navigate('/update-profile');
    }

    const handleLogout = () => {
        localStorage.clear();
        googleLogout();
        navigate('/');
    }

    const handleBackToHome = () => {
        if (DecodeToken()!.user_role === 'User') {
            navigate('/user/dashboard');
          } else if (DecodeToken()!.user_role === 'Driver') {
              navigate('/driver/dashboard');
          } else if (DecodeToken()!.user_role === 'Admin') {
              navigate('/admin/dashboard');
          }
    }
    
    return(
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <span className={`navbar-brand ${styles.home} ${styles.link}`} onClick={handleBackToHome}>UBER</span>
        <span className={`nav-item nav-link disabled ${styles.status} ms-4`}>{DecodeToken()?.verification}</span>
        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNavAltMarkup" aria-controls="navbarNavAltMarkup" aria-expanded="false" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNavAltMarkup">
            <div className="navbar-nav ms-auto">
                <span className={`nav-item nav-link ${styles.link}`} onClick={handleUpdateProfile}>Update Profile</span>
                <span className={`nav-item nav-link ${styles.link}`} onClick={handleLogout}>Logout</span>
            </div>
        </div>
        </nav>
    )
}