import { Navigate } from 'react-router-dom';
import { DecodeToken } from '../../services/token-decoder';

function RequireRole({ children, role }) {

  if (DecodeToken()?.user_role !== role) {
    return <Navigate to="/" />;
  }

  return children;
}

export default RequireRole;
