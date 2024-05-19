import { Formik, Field, ErrorMessage, FormikValues, FormikHelpers } from "formik";
import * as Yup from "yup";
import { register } from "../../services/user.service";
import styles from "../register/register.module.css";
import { useNavigate } from "react-router-dom";
import { UserModel } from "../../shared/models/user";

export default function Register() {
  const navigate = useNavigate();
  interface RegisterFormValues {
    userName: string;
    email: string;
    password: string;
    name: string;
    lastName: string;
    birthday: string;
    address: string;
    role: boolean;
    image: string;
  }
  
  const initialValues: RegisterFormValues = {
    userName: "",
    email: "",
    password: "",
    name: "",
    lastName: "",
    birthday: "",
    address: "",
    role: false,
    image: "",
  };

  const validationSchema = Yup.object().shape({
    userName: Yup.string().required("Username is required"),
    email: Yup.string().email("Invalid email").required("Email is required"),
    password: Yup.string().required("Password is required"),
    name: Yup.string().required("Name is required"),
    lastName: Yup.string().required("Last name is required"),
    birthday: Yup.string().required("Date of birth is required"),
    address: Yup.string().required("Address is required"),
    image: Yup.string().required("Image is required"),
  });

  const handleSubmit = async (values: FormikValues, { setSubmitting }: FormikHelpers<RegisterFormValues>) => {
    var role = values.role;
    values.role = values.role ? 2 : 1;
    try {
      await register(values as UserModel);
      navigate("/");
    } catch {
      values.role = role;
    }
    setSubmitting(false);
  };

  return (
    <div className={styles.wrapper}>
      <div className={styles.form}>
        <h2 className="mb-4">Register</h2>
        <Formik initialValues={initialValues} validationSchema={validationSchema} onSubmit={handleSubmit}>
          {({ isValid, dirty, errors, touched, handleSubmit }) => (
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label htmlFor="userName">Username</label>
                <Field type="text" className={`form-control ${styles.field} ${errors.userName && touched.userName ? styles.inputError : ""}`} name="userName" placeholder="Enter username" />
                <ErrorMessage name="userName" component="div" className={styles.error} />
              </div>
              <div className="form-group">
                <label htmlFor="email">Email address</label>
                <Field type="email" className={`form-control ${styles.field} ${errors.email && touched.email ? styles.inputError : ""}`} name="email" placeholder="Enter email" />
                <ErrorMessage name="email" component="div" className={styles.error} />
              </div>
              <div className="form-group">
                <label htmlFor="password">Password</label>
                <Field type="password" className={`form-control ${styles.field} ${errors.password && touched.password ? styles.inputError : ""}`} name="password" placeholder="Password" />
                <ErrorMessage name="password" component="div" className={styles.error} />
              </div>
              <div className="form-group">
                <label htmlFor="name">Name</label>
                <Field type="text" className={`form-control ${styles.field} ${errors.name && touched.name ? styles.inputError : ""}`} name="name" placeholder="Enter name" />
                <ErrorMessage name="name" component="div" className={styles.error} />
              </div>
              <div className="form-group">
                <label htmlFor="lastName">Last Name</label>
                <Field type="text" className={`form-control ${styles.field} ${errors.lastName && touched.lastName ? styles.inputError : ""}`} name="lastName" placeholder="Enter last name" />
                <ErrorMessage name="lastName" component="div" className={styles.error} />
              </div>
              <div className="form-group">
                <label htmlFor="birthday">Date of Birth</label>
                <Field type="text" className={`form-control ${styles.field} ${errors.birthday && touched.birthday ? styles.inputError : ""}`} name="birthday" placeholder="YYYY-MM-DD" />
                <ErrorMessage name="birthday" component="div" className={styles.error} />
              </div>
              <div className="form-group">
                <label htmlFor="address">Address</label>
                <Field type="text" className={`form-control ${styles.field} ${errors.address && touched.address ? styles.inputError : ""}`} name="address" placeholder="Enter address" />
                <ErrorMessage name="address" component="div" className={styles.error} />
              </div>
              <div className="form-group">
                <Field type="checkbox" className={`form-check-input`} name="role" />
                <label htmlFor="role" className="form-check-label ms-2">
                  Are you a driver?
                </label>
              </div>
              <div className="form-group">
                <label htmlFor="image">Profile Picture</label>
                <Field type="text" className={`form-control ${styles.field} ${errors.image && touched.image ? styles.inputError : ""}`} name="image" placeholder="Enter image URL" />
                <ErrorMessage name="image" component="div" className={styles.error} />
              </div>
              <button type="submit" className={`btn btn-primary mt-3 ${styles.submitButton}`} disabled={!isValid || !dirty}>
                Submit
              </button>
            </form>
          )}
        </Formik>
      </div>
    </div>
  );
}
