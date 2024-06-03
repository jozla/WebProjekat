import { Formik, Field, ErrorMessage, FormikValues, FormikHelpers } from "formik";
import * as Yup from "yup";
import { login, register } from "../../services/user.service";
import { Link, useNavigate } from "react-router-dom";
import styles from "../login/login.module.css";
import { DecodeToken } from "../../services/token-decoder";
import { LoginFormValues } from "./login";
import { GoogleLogin } from "@react-oauth/google";
import { jwtDecode } from "jwt-decode";
import { UserModel } from "../../shared/models/user";

export default function LogIn() {
  const navigate = useNavigate();

  const initialValues: LoginFormValues = {
    email: "",
    password: "",
  };

  const validationSchema = Yup.object().shape({
    email: Yup.string().email("Invalid email").required("Email is required"),
    password: Yup.string().required("Password is required"),
  });

  const handleSubmit = async (values: FormikValues, { setSubmitting }: FormikHelpers<LoginFormValues>) => {
    try {
      var response = await login(values as LoginFormValues);
      if (response.token) {
        localStorage.setItem("access_token", JSON.stringify(response.token));
        var decodedToken = DecodeToken()!;
        if (decodedToken.user_role === "User") {
          navigate("user/dashboard");
        } else if (decodedToken.user_role === "Driver") {
          navigate("driver/dashboard");
        } else if (decodedToken.user_role === "Admin") {
          navigate("admin/dashboard");
        }
      }
    } catch {}
    setSubmitting(false);
  };

  interface TokenPayload {
    given_name: string;
    family_name: string;
    email: string;
  }
  
  const googleLogIn = (token: string) => {
    localStorage.setItem("access_token", JSON.stringify(token));
    navigate("user/dashboard");
  }

  const responseMessage = async (response) => {
    var decodedToken = jwtDecode<TokenPayload>(response.credential);
    try{
      var loginResponse = await login({ email: decodedToken.email, password: "" });
      if (loginResponse.token) {
        googleLogIn(loginResponse.token);
      }
    }
    catch{
      var data = {
        userName: decodedToken.email,
        email: decodedToken.email,
        password: "",
        name: decodedToken.given_name,
        lastName: decodedToken.family_name,
        birthday: "",
        address: "",
        role: 1,
        image: null,
      };
      await register(data as UserModel);
      response = await login({ email: decodedToken.email, password: "" });
      googleLogIn(response.token);
    }
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
                <Field type="email" className={`form-control ${styles.field} ${errors.email && touched.email ? styles.inputError : ""}`} name="email" placeholder="Enter email" />
                <ErrorMessage name="email" component="div" className={styles.error} />
              </div>
              <div className="form-group">
                <label htmlFor="password">Password</label>
                <Field type="password" className={`form-control  ${styles.field} ${errors.password && touched.password ? styles.inputError : ""}`} name="password" placeholder="Password" />
                <ErrorMessage name="password" component="div" className={styles.error} />
              </div>
              <button type="submit" className={`btn btn-dark mt-3 ${styles.submitButton}`} disabled={!isValid || !dirty}>
                Submit
              </button>
            </form>
          )}
        </Formik>
        <p>
          Don't have account? Sign in <Link to="/register">here</Link>
        </p>
        <GoogleLogin onSuccess={responseMessage} />
      </div>
    </div>
  );
}
