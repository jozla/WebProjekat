import { RatingModel } from "../shared/models/rating";
import axiosInstance from "./axios-interceptor";

export const addRating = async (data: RatingModel) => {
    try {
      await axiosInstance.post("/ratings", data);
    } catch (error) {
      console.error("Error fetching rating:", error);
      throw error;
    }
};

export const getRating = async (userId: string) => {
  try {
    const response = await axiosInstance.get("/ratings/"+userId);
    return response.data;
  } catch (error) {
    console.error("Error fetching rating:", error);
    throw error;
  }
};