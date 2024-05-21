import { Navigate } from 'react-router-dom';
import { DecodeToken } from '../../services/token-decoder';

function RequireAuth({ children }) {

  var decodedToken = DecodeToken();
  if (!decodedToken) {
    return <Navigate to="/" />;
  }

  return children;
}

export default RequireAuth;
