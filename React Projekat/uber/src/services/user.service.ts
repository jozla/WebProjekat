import { LoginFormValues } from "../features/login/login";
import { UpdateFormValues } from "../features/update-profile/update";
import { UserModel } from "../shared/models/user";
import axiosInstance from "./axios-interceptor";

export const login = async (data: LoginFormValues) => {
  try {
    const response = await axiosInstance.post("/users/login", data);
    return response.data;
  } catch (error) {
    console.error("Error fetching users:", error);
    throw error;
  }
};

export const register = async (data: UserModel) => {
  try {
    var formData = new FormData();
    Object.keys(data).forEach((key) => {
      formData.append(key, data[key]);
    });
    await axiosInstance.post("/users", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  } catch (error) {
    console.error("Error fetching users:", error);
    throw error;
  }
};

export const updateProfile = async (data: UpdateFormValues) => {
  try {
    var formData = new FormData();
    Object.keys(data).forEach((key) => {
      formData.append(key, data[key]);
    });
    await axiosInstance.put("/users", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  } catch (error) {
    console.error("Error fetching users:", error);
    throw error;
  }
};

export const getUserById = async (userId: string) => {
  try {
    const response = await axiosInstance.get("/users/" + userId);
    return response.data;
  } catch (error) {
    console.error("Error fetching users:", error);
    throw error;
  }
};

export const getAllDrivers = async () => {
  try {
    const response = await axiosInstance.get("/users/get-drivers");
    return response.data;
  } catch (error) {
    console.error("Error fetching users:", error);
    throw error;
  }
};

export const verifyUser = async (data: any) => {
  try {
    await axiosInstance.post("/users/verify", data);
  } catch (error) {
    console.error("Error fetching users:", error);
    throw error;
  }
};

export const blockUser = async (data: any) => {
  try {
    await axiosInstance.post("/users/block", data);
  } catch (error) {
    console.error("Error fetching users:", error);
    throw error;
  }
};
