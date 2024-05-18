import { Formik, Field, ErrorMessage, FormikValues, FormikHelpers } from "formik";
import * as Yup from "yup";
import { login } from "../../services/user.service";
import { Link, useNavigate } from "react-router-dom";
import styles from '../login/login.module.css'
import { DecodeToken } from "../../services/token-decoder";

export default function LogIn() {
  const navigate = useNavigate();
  interface LoginFormValues {
    email: string;
    password: string;
  }
  
  const initialValues: LoginFormValues = {
    email: "",
    password: "",
  };

  const validationSchema = Yup.object().shape({
    email: Yup.string().email("Invalid email").required("Email is required"),
    password: Yup.string().required("Password is required"),
  });

  const handleSubmit = async (values: FormikValues, { setSubmitting }: FormikHelpers<LoginFormValues>) => {
    var response = await login(values);
    localStorage.setItem("access_token",JSON.stringify(response.token));

    //TODO
    // var decodedToken = DecodeToken();
    navigate('user/add-ride')

    setSubmitting(false);
  };

  return (
    <div className={styles.wrapper}>
      <div className={styles.form}>
        <h2 className="mb-4">Log In or Sign Up</h2>
        <Formik initialValues={initialValues} validationSchema={validationSchema} onSubmit={handleSubmit}>
          {({ isValid, dirty, errors, touched, handleSubmit }) => (
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label htmlFor="email">Email address</label>
                <Field
                  type="email"
                  className={`form-control ${styles.field} ${errors.email && touched.email ? styles.inputError : ""}`}
                  name="email"
                  placeholder="Enter email"
                />
                <ErrorMessage name="email" component="div" className={styles.error} />
              </div>
              <div className="form-group">
                <label htmlFor="password">Password</label>
                <Field
                  type="password"
                  className={`form-control  ${styles.field} ${errors.password && touched.password ? styles.inputError : ""}`}
                  name="password"
                  placeholder="Password"
                />
                <ErrorMessage name="password" component="div" className= {styles.field} />
              </div>
              <button type="submit" className={`btn btn-primary mt-3 ${styles.submitButton}`} disabled={!isValid || !dirty}>
                Submit
              </button>
            </form>
          )}
        </Formik>
        <p>Don't have account? Sign in <Link to='/register'>here</Link></p>
      </div>
    </div>
  );
}
