import { Formik, Field, ErrorMessage, FormikValues, FormikHelpers } from "formik";
import * as Yup from "yup";
import { getUserById, updateProfile } from "../../services/user.service";
import styles from "../update-profile/update-profile.module.css";
import { useEffect, useState } from "react";
import { DecodeToken } from "../../services/token-decoder";
import { UpdateFormValues } from "./update";
import { Header } from "../../shared/header/header";

export default function UpdateProfile() {
  const [initialValues, setInitialValues] = useState<UpdateFormValues | null>(null);
  const [image, setImage] = useState<any>(null);
  const [imageUrl, setImageUrl] = useState<string>('');

  useEffect(() => {
    getUserData();
  }, []);

  const getUserData = async () => {
    setInitialValues(null);
    var response = await getUserById(DecodeToken()!.user_id);
    setInitialValues({
      id: response.user.id,
      userName: response.user.userName,
      email: response.user.email,
      oldPassword: "",
      newPassword: "",
      name: response.user.name,
      lastName: response.user.lastName,
      birthday: response.user.birthday,
      address: response.user.address,
      image: null,
    });
   
    setImageUrl(`http://localhost:8389/images/${response.user.image}`);
  };

  const validationSchema = Yup.object().shape({
    userName: Yup.string().required("Username is required"),
    email: Yup.string().email("Invalid email").required("Email is required"),
    oldPassword: Yup.string().required("Old password is required"),
    newPassword: Yup.string().required("New Password is required"),
    name: Yup.string().required("Name is required"),
    lastName: Yup.string().required("Last name is required"),
    birthday: Yup.string()
      .required("Date of birth is required")
      .matches(/^\d{4}-\d{2}-\d{2}$/, "Date format must be yyyy-mm-dd"),
    address: Yup.string().required("Address is required"),
  });

  const handleSubmit = async (values: FormikValues, { setSubmitting }: FormikHelpers<UpdateFormValues>) => {
    try {
      values.image = image;
      await updateProfile(values as UpdateFormValues);
      getUserData();
    } catch {}
    setSubmitting(false);
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    setImage(file);
  };

  if (!initialValues) {
    return <div>Loading...</div>;
  }

  return (
    <>
      <Header></Header>
      <div className={styles.wrapper}>
        <div className={styles.form}>
            <h2 className="mb-4">Update profile</h2>
          <p>If you did registration via Google in oldPassword or newPassword field you can just type "/" if you don't want to change password.</p>
          <div className={styles.profileImage}>
            <p>Profile image:</p>
            <img className={styles.image} src={imageUrl} alt='Missing' />
          </div>
          <Formik initialValues={initialValues!} validationSchema={validationSchema} onSubmit={handleSubmit}>
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
                  <label htmlFor="oldPassword">Old Password</label>
                  <Field type="password" className={`form-control ${styles.field} ${errors.oldPassword && touched.oldPassword ? styles.inputError : ""}`} name="oldPassword" placeholder="Old Password" />
                  <ErrorMessage name="oldPassword" component="div" className={styles.error} />
                </div>
                <div className="form-group">
                  <label htmlFor="newPassword">New Password</label>
                  <Field type="password" className={`form-control ${styles.field} ${errors.newPassword && touched.newPassword ? styles.inputError : ""}`} name="newPassword" placeholder="New Password" />
                  <ErrorMessage name="newPassword" component="div" className={styles.error} />
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
                <label htmlFor="image">Image</label>
                  <input type="file" name="image" accept="image/*" className={`form-control ${styles.field}`} onChange={handleImageChange} required />
                </div>
                <button type="submit" className={`btn btn-dark mt-3 ${styles.submitButton}`} disabled={!isValid || !dirty}>
                  Submit
                </button>
              </form>
            )}
          </Formik>
        </div>
      </div>
    </>
  );
}
