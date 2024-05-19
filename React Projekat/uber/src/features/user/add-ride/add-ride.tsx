import { ErrorMessage, Field, Formik, FormikHelpers, FormikValues } from "formik";
import styles from "../add-ride/add-ride.module.css";
import * as Yup from "yup";
import { DecodeToken } from "../../../services/token-decoder";
import { useState } from "react";
import { addRide } from "../../../services/ride.service";
import { RideModel } from "../../../shared/models/ride";

export function AddRide() {
  const [rideDetails, setRideDetails] = useState<RideModel|null>(null);

  interface NewRideValues {
    startingPoint: string;
    endingPoint: string;
  }

  const initialValues = {
    startingPoint: "",
    endingPoint: "",
  };

  const validationSchema = Yup.object().shape({
    startingPoint: Yup.string().required("Starting point is required"),
    endingPoint: Yup.string().required("Ending point is required"),
  });

  const generateRandomValues = () => {
    var randomPrice = Math.floor(Math.random() * 100) + 1; 
    var randomDriverTime = Math.floor(Math.random() * 3600); 
    randomDriverTime = Math.max(randomDriverTime, 300);
    return { price: randomPrice, driverTime: randomDriverTime };
  };

  const handleSubmit = async (values: FormikValues, { setSubmitting }: FormikHelpers<NewRideValues>) => {
    const { price, driverTime } = generateRandomValues();
    const rideData = {
      startingPoint: values.startingPoint,
      endingPoint: values.endingPoint,
      price: price,
      driverTimeInSeconds: driverTime,
      passengerId: DecodeToken().user_id,
    };
    setRideDetails(rideData as RideModel);
    setSubmitting(false);
  };

  const confirmRide = async () => {
    await addRide(rideDetails as RideModel);
  };

  const OnChange = () => {
    setRideDetails(null);
  };

  return (
    <div className={styles.wrapper}>
      <div className={styles.form}>
        <h2 className="mb-4">Chose your ride now!</h2>
        <Formik initialValues={initialValues} validationSchema={validationSchema} onSubmit={handleSubmit}>
          {({ isValid, dirty, errors, touched, isSubmitting, handleSubmit }) => (
            <form onSubmit={handleSubmit} onChange={OnChange}>
              <div className="form-group">
                <label htmlFor="startingPoint">Starting Point</label>
                <Field
                  type="text"
                  className={`form-control ${styles.field} ${errors.startingPoint && touched.startingPoint ? styles.inputError : ""}`}
                  name="startingPoint"
                  placeholder="Enter starting point"
                />
                <ErrorMessage name="startingPoint" component="div" className={styles.error} />
              </div>
              <div className="form-group">
                <label htmlFor="endingPoint">Ending Point</label>
                <Field
                  type="text"
                  className={`form-control ${styles.field} ${errors.endingPoint && touched.endingPoint ? styles.inputError : ""}`}
                  name="endingPoint"
                  placeholder="Enter ending point"
                />
                <ErrorMessage name="endingPoint" component="div" className={styles.error} />
              </div>
              <button type="submit" className={`btn btn-primary mt-3 ${styles.submitButton}`} disabled={!isValid || !dirty || isSubmitting}>
                Submit
              </button>
            </form>
          )}
        </Formik>
        {rideDetails && (
          <div className={`mt-3 ${styles.confirmRideDiv}`}>
            <p><span className={styles.bold}>Driver's arrival time:</span> {Math.floor(rideDetails.driverTimeInSeconds / 60)} minutes</p>
            <p><span className= {styles.bold}>Price:</span> ${rideDetails.price}</p>
            <button className={`btn btn-primary ${styles.submitButton}`} onClick={confirmRide}>
              Confirm ride
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
