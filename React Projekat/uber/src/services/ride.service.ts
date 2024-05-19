import { RideModel } from "../shared/models/ride";
import axiosInstance from "./axios-interceptor";

export const addRide = async (data: RideModel) => {
  try {
    await axiosInstance.post("/rides", data);
  } catch (error) {
    console.error("Error fetching rides:", error);
    throw error;
  }
};

export const getUserRides = async (userId:string) => {
  try {
    const response = await axiosInstance.get("/rides/user-rides/" + userId);
    return response.data;
  } catch (error) {
    console.error("Error fetching rides:", error);
    throw error;
  }
}

export const getPreviousRidesForDriver = async (driverId:string) => {
  try {
    const response = await axiosInstance.get("/rides/driver-rides/" + driverId);
    return response.data;
  } catch (error) {
    console.error("Error fetching rides:", error);
    throw error;
  }
}

export const getNewRides = async () => {
  try {
    const response = await axiosInstance.get("/rides/new-rides/");
    return response.data;
  } catch (error) {
    console.error("Error fetching rides:", error);
    throw error;
  }
}

export const confirmRide = async (data:any) => {
  try {
    await axiosInstance.put("/rides/confirm-ride/", data);
  } catch (error) {
    console.error("Error fetching rides:", error);
    throw error;
  }
}
