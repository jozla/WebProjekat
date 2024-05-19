import { LoginFormValues } from "../features/login/login";
import { UserModel } from "../shared/models/user";
import axiosInstance from "./axios-interceptor";

export const login = async (data:LoginFormValues) => {
  try {
    const response = await axiosInstance.post('/users/login', data);
    return response.data;
  } catch (error) {
    console.error("Error fetching users:", error);
    throw error;
  }
};

export const register = async (data:UserModel) => {
    try {
      console.log(data)
      await axiosInstance.post('/users', data);
    } catch (error) {
      console.error("Error fetching users:", error);
      throw error;
    }
};
