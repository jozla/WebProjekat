import { jwtDecode } from "jwt-decode";

interface TokenPayload {
    user_role: string;
    user_id: string;
    verification: string;
  }

export function DecodeToken(){
    return jwtDecode<TokenPayload>(localStorage.getItem('access_token')!)
}