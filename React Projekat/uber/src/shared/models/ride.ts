export interface RideModel {
    id: string;
    startingPoint: string;
    endingPoint: string;
    price: number;
    driverTimeInSeconds: number;
    arrivalTimeInSeconds: number;
    driverId: string;
    passengerId: string;
    status: number;
}