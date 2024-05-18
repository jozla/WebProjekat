import axiosInstance from "./axios-interceptor";

export const addRide = async (data: any) => {
  try {
    await axiosInstance.post("/rides", data);
  } catch (error) {
    console.error("Error fetching rides:", error);
    throw error;
  }
};

export const getUserRides = async (userId:any) => {
  try {
    const response = await axiosInstance.get("/rides/user-rides/" + userId);
    return response.data;
  } catch (error) {
    console.error("Error fetching rides:", error);
    throw error;
  }
}
