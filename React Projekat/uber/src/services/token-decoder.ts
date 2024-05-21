import { jwtDecode } from "jwt-decode";

interface TokenPayload {
    user_role: string;
    user_id: string;
    verification: string;
}

export function DecodeToken(){
    if(!localStorage.getItem('access_token')){
      return null
    }
    var token = jwtDecode<TokenPayload>(localStorage.getItem('access_token')!);
    if(token){
      return token; 
    }
    else{
      return null;
    }
}