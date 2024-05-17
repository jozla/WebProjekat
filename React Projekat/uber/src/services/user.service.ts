import axiosInstance from "./axios-interceptor";

export const login = async (data:any) => {
  try {
    const response = await axiosInstance.post('/users/login', data);
    return response.data;
  } catch (error) {
    console.error("Error fetching users:", error);
    throw error;
  }
};

export const register = async (data:any) => {
    try {
      await axiosInstance.post('/users', data);
    } catch (error) {
      console.error("Error fetching users:", error);
      throw error;
    }
  };
