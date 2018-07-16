/////////////////////////////////////////////////////////////////////////////////
////
////  Original System: ur5_kinematics.cs
////  Subsystem:       Human-Robot Interaction
////  Workfile:        Standalone
////  Revision:        1.1 - 6/26/2018
////  Authors :        Shelly Bagachi
////                   Esteban Segarra
////
////  Description
////  ===========
////  UR5 Controller for IK control
////
/////////////////////////////////////////////////////////////////////////////////
//using UnityEngine;
//using UnityEngine.UI;
//using MathNet.Numerics.LinearAlgebra;
////using MathNet.Numerics.LinearAlgebra.Double;
//using System;
//using System.Collections.Generic;

//public class ur5_kinematics_old : MonoBehaviour {
//    private Button thisButton;
//    public  InputField outputText;

//    public  Button testButton;
//    public  Button goButton;
//    public Button get_IK_button; 
//    public  GameObject locationSphere;

//    private ur5 robotModel;

   

//    public  UR5Controller controller;
//    public  GetFromServer gfs;
//    private GameObject[] jointList = new GameObject[6];
//    private Slider[] sliderList = new Slider[6];


//    Matrix<float> robotTheta_filt;




//    Matrix<float> convert_4x4_to_matrix(Matrix4x4 input)
//    {
//        Matrix<float> manipulator = Matrix<float>.Build.DenseOfArray(new float[,] {
//        { input.m00, input.m01, input.m02, input.m03},
//        { input.m10, input.m11, input.m12, input.m13},
//        { input.m20, input.m21, input.m22, input.m23},
//        { input.m30, input.m31, input.m32, input.m33},
//        });

//        //Debug.Log(manipulator + "HEYO");
//        return manipulator;
//    }

//    // Use this for initialization
//    void Start () {
//        //thisButton = this.GetComponent<Button>();
//        get_IK_button.onClick.AddListener(TaskOnClick);

//        testButton.onClick.AddListener(TaskOnClick2);
//        goButton.onClick.AddListener(TaskOnClick3);

//        robotModel = new ur5();

//        controller.initializeJoints(jointList);
//        controller.initializeSliders(sliderList);

//    }
	
//	// Update is called once per frame
//	void Update () {
//        return;
//	}


//    //  Called when get-IK button is clicked
//    void TaskOnClick() {
//        // Get OFFSET values from sliders and run FK to get pose
//        Vector<float> jointVals = Vector<float>.Build.DenseOfArray(controller.offsetSliderValues(controller.getSliderList()));
//        Matrix<float> robotPose = robotModel.fwd_kin(jointVals.Multiply(( Mathf.PI  / 180f)));  // in rads
//       // Debug.Log("Input Robot Pose: " + robotPose.ToString(4,4));
//        robotModel.setCurrentJoints(jointVals.Multiply((Mathf.PI / 180f)));

//        // Use pose to run IK
//        Matrix<float> robotTheta = robotModel.inv_kin(robotPose);
//        //Debug.Log("Theta from IK: " + robotTheta.ToString(6, 8));
//       // Debug.Log("Theta from IK: " + robotTheta.Multiply((180f / Mathf.PI)).ToString(6, 8));

//        // Filter results using EE pose and joint limits
//        robotModel.setLimits(controller.upperLimit_r, controller.lowerLimit_r);
//        robotTheta_filt = robotModel.filter_invKin(robotTheta, robotPose);
//        //Debug.Log("Filtered theta: " + robotTheta_filt.ToString(6, 8));
//        //Debug.Log("Filtered theta: " + robotTheta_filt.Multiply((180f / Mathf.PI)).ToString(6, 8));

//        // Display in text field
//        outputText.text = robotTheta_filt.Column(0).Multiply((180f / Mathf.PI)).ToVectorString("0.0  ");
//    }

//    //  Called when go-to-IK TEST button is clicked
//    void TaskOnClick2()
//    {
//        var soln = robotTheta_filt.Column(0).Multiply((180f / Mathf.PI));
//        //Debug.Log(soln);
//        controller.setSliderList(controller.offsetJointValues(soln.ToArray()));
//    }

//    //  Called when go-to-IK button (SPHERE) is clicked
//    void TaskOnClick3()
//    {
//        //Locate the transform matrix of the pointer sphere 
//        Matrix4x4 positional_matrix = locationSphere.transform.localToWorldMatrix;
//        Debug.Log(positional_matrix);

//        Matrix<float> pose_matrix = convert_4x4_to_matrix(positional_matrix);
//        Debug.Log(pose_matrix  );
//        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAA");
//        //Locate data from Virtual robot and pose it, run FK. Configure the IK system
//        Vector<float> jointVals = Vector<float>.Build.DenseOfArray(controller.offsetSliderValues(controller.getSliderList()));
//        Matrix<float> robotPose = robotModel.fwd_kin(jointVals.Multiply((Mathf.PI / 180f)));  // in rads
//        robotModel.setCurrentJoints(jointVals.Multiply((Mathf.PI / 180f)));
//        robotModel.setLimits(controller.upperLimit_r, controller.lowerLimit_r);

       
//        //var targetPos = locationSphere.transform.position;
        
       
//        Matrix<float> matrix_thetha = robotModel.inv_kin(pose_matrix);
//        //Debug.Log(matrix_thetha);

//        //Create the matrix thetha
//        matrix_thetha = robotModel.filter_invKin(matrix_thetha, robotPose);
//        //Debug.Log(matrix_thetha);

//        //Output the kinematics to the virtual robot
//        var soln = matrix_thetha.Column(0).Multiply((180f / Mathf.PI));
//        //Debug.Log(soln);

//        controller.setSliderList(controller.offsetJointValues(soln.ToArray()));

//        outputText.text = matrix_thetha.Column(0).Multiply((180f / Mathf.PI)).ToVectorString("0.0  ");
//        //Debug.Log(soln);
//        // How to get pose matrix from location??
//        // Get dist from EE and conversion factor from robot model size?

//    }

//   //  Converted python code from SK Gupta's lab
//    public class ur5
//    {
//        private Vector<float> a = Vector<float>.Build.Dense(6);
//        private Vector<float> d = Vector<float>.Build.Dense(6);
//        private Vector<float> alpha = Vector<float>.Build.Dense(6);
//        private Matrix<float> dh = Matrix<float>.Build.Dense(4, 4);

//        private Vector<float> upperLimits = Vector<float>.Build.Dense(6);
//        private Vector<float> lowerLimits = Vector<float>.Build.Dense(6);
//        private Vector<float> prevJoints = Vector<float>.Build.Dense(6);


//        public ur5()
//        {
//            // a:  length offset between joints (e.g. length of arm segments in m)
//            a.SetValues(new float[] { 0, -0.425f, -0.39225f, 0, 0, 0 });
//            d.SetValues(new float[] { 0.089159f, 0, 0, 0.10915f, 0.09465f, 0.0823f });
//            alpha.SetValues(new float[] { Mathf.PI / 2, 0, 0, Mathf.PI / 2, -1 * Mathf.PI / 2, 0 });
//        }
        

//        public void setLimits(float[] upper, float[] lower)
//        {
//            upperLimits = Vector<float>.Build.DenseOfArray(upper);
//            lowerLimits = Vector<float>.Build.DenseOfArray(lower);
//            // Convert to rads
//            upperLimits = upperLimits.Multiply((Mathf.PI / 180f));
//            lowerLimits = lowerLimits.Multiply((Mathf.PI / 180f));
//        }

//        public void setCurrentJoints(Vector<float> joints)
//        {
//            prevJoints = joints;
//        }



//        private Matrix<float> DH(float aa, float aalpha, float dd, float ttheta) {
//            dh = Matrix<float>.Build.DenseOfArray(new float[,] {
//                { Mathf.Cos(ttheta), -1 * Mathf.Sin(ttheta) * Mathf.Cos(aalpha), Mathf.Sin(ttheta) * Mathf.Sin(aalpha), aa* Mathf.Cos(ttheta) },
//                { Mathf.Sin(ttheta), Mathf.Cos(ttheta) * Mathf.Cos(aalpha), -1 * Mathf.Cos(ttheta) * Mathf.Sin(aalpha), aa* Mathf.Sin(ttheta) },
//                { 0, Mathf.Sin(aalpha), Mathf.Cos(aalpha), dd},
//                { 0, 0, 0, 1} });


//            for (int i = 0; i < dh.RowCount; i++) {
//                for (int j = 0; j < dh.ColumnCount; j++) {
//                    if (Mathf.Abs(dh[i, j]) < 0.0001f)
//                        dh[i, j] = 0.0f;
//                }
//            }

//            return dh;
//        }

//        // in rads!
//        public Matrix<float> fwd_kin(Vector<float> joints) {
//            var T01 = DH(a[0], alpha[0], d[0], joints[0]);
//            var T12 = DH(a[1], alpha[1], d[1], joints[1]);
//            var T23 = DH(a[2], alpha[2], d[2], joints[2]);
//            var T34 = DH(a[3], alpha[3], d[3], joints[3]);
//            var T45 = DH(a[4], alpha[4], d[4], joints[4]);
//            var T56 = DH(a[5], alpha[5], d[5], joints[5]);

//            //return Matrix<float>.op_DotMultiply(np.dot(np.dot(np.dot(np.dot(T01, T12), T23), T34), T45), T56);
//            return (((((T01 * T12) * T23) * T34) * T45) * T56);
//        }


//        public Matrix<float> inv_kin(Matrix<float> pose) {
//            // pose is the 4x4 matrix of the end effector
//            // DH parameters
//            //theta = np.zeros((6, 8))
//            Matrix<float> theta = Matrix<float>.Build.Dense(6, 8);

//            // theta1
//            var temp1 = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, -d[5], 1 });
//            var temp2 = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 1 });
//            var p05 = (pose * temp1) - temp2;
//            var psi = Mathf.Atan2(p05[1], p05[0]);
//            var phi = 0f;
//            if (d[3] / Mathf.Sqrt(p05[1] * p05[1] + p05[0] * p05[0]) > 1)
//                phi = 0f;
//            else
//                phi = Mathf.Acos(d[3] / Mathf.Sqrt(p05[1] * p05[1] + p05[0] * p05[0]));
//            //theta[0, :4] = m.pi / 2 + psi + phi;
//            theta.SetSubMatrix(0, 0, Matrix<float>.Build.Dense(1, 4, Mathf.PI / 2 + psi + phi));
//            //theta[0, 4:8] = Mathf.PI / 2 + psi - phi;
//            theta.SetSubMatrix(0, 4, Matrix<float>.Build.Dense(1, 4, Mathf.PI / 2 + psi - phi));

//            // theta5
//            for (int c = 0; c < 4; c++) {
//                var T10 = DH(a[0], alpha[0], d[0], theta[0, c]).Inverse();
//                var T16 = (T10 * pose);
//                var p16z = T16[2, 3];
//                var t5 = 0f;
//                if ((p16z - d[3]) / d[5] > 1)
//                    t5 = 0f;
//                else
//                    t5 = Mathf.Acos((p16z - d[3]) / d[5]);

//                //theta[4, c: c + 1 + 1] = t5;
//                //theta[4, c + 2:c + 3 + 1] = -t5;
//                theta[4, c] = t5;
//                theta[4, c + 1] = t5;
//                theta[4, c + 2] = -t5;
//                theta[4, c + 3] = -t5;
//            }

//            // theta6
//            for (int c = 0; c <= 6 && c % 2 == 0; c++) {
//                var T01 = DH(a[0], alpha[0], d[0], theta[0, c]);
//                var T61 = pose.Inverse() * T01;
//                var T61zy = T61[1, 2];
//                var T61zx = T61[0, 2];
//                var t5 = theta[4, c];
//                //theta[5, c: c + 1 + 1] = Mathf.Atan2(-T61zy / Mathf.Sin(t5), T61zx / Mathf.Sin(t5));
//                theta[5, c] = Mathf.Atan2(-T61zy / Mathf.Sin(t5), T61zx / Mathf.Sin(t5));
//                theta[5, c + 1] = Mathf.Atan2(-T61zy / Mathf.Sin(t5), T61zx / Mathf.Sin(t5));
//            }

//            // theta3
//            for (int c = 0; c <= 6 && c % 2 == 0; c++) {
//                var T10 = DH(a[0], alpha[0], d[0], theta[0, c]).Inverse();
//                var T65 = DH(a[5], alpha[5], d[5], theta[5, c]).Inverse();
//                var T54 = DH(a[4], alpha[4], d[4], theta[4, c]).Inverse();
//                var T14 = ((T10 * pose) * (T65 * T54));
//                temp1 = Vector<float>.Build.DenseOfArray(new float[] { 0, -d[3], 0, 1 });
//                temp2 = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 1 });
//                var p13 = (T14 * temp1) - temp2;
//                //var p13norm2 = la.norm(p13) * *2;  // Frobenius norm
//                var p13norm2 = p13.Norm(2) * p13.Norm(2);  // L2 norm is same?
//                var t3p = 0f;
//                if ((p13norm2 - a[1] * a[1] - a[2] * a[2]) / (2 * a[1] * a[2]) > 1)
//                    t3p = 0f;
//                else
//                    t3p = Mathf.Acos(((float)p13norm2 - a[1] * a[1] - a[2] * a[2]) / (2 * a[1] * a[2]));
//                theta[2, c] = t3p;
//                theta[2, c + 1] = -t3p;
//            }

//            // theta2 theta4
//            for (int c = 0; c < 8; c++) {
//                var T10 = DH(a[0], alpha[0], d[0], theta[0, c]).Inverse();
//                var T65 = DH(a[5], alpha[5], d[5], theta[5, c]).Inverse();
//                var T54 = DH(a[4], alpha[4], d[4], theta[4, c]).Inverse();
//                var T14 = ((T10 * pose) * (T65 * T54));
//                temp1 = Vector<float>.Build.DenseOfArray(new float[] { 0, -d[3], 0, 1 });
//                temp2 = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 1 });
//                var p13 = (T14 * temp1) - temp2;
//                var p13norm = p13.Norm(2);
//                theta[1, c] = -Mathf.Atan2(p13[1], -p13[0]) + Mathf.Asin(a[2] * Mathf.Sin(theta[2, c]) / (float)p13norm);
//                var T32 = DH(a[2], alpha[2], d[2], theta[2, c]).Inverse();
//                var T21 = DH(a[1], alpha[1], d[1], theta[1, c]).Inverse();
//                var T34 = ((T32 * T21) * T14);
//                theta[3, c] = Mathf.Atan2(T34[1, 0], T34[0, 0]);
//            }

//            for (int i = 0; i < theta.RowCount; i++) {
//                for (int j = 0; j < theta.ColumnCount; j++) {
//                    if (theta[i, j] > Mathf.PI)
//                        theta[i, j] = theta[i, j] - 2 * Mathf.PI;
//                    if (theta[i, j] < -1 * Mathf.PI)
//                        theta[i, j] = theta[i, j] + 2 * Mathf.PI;
//                }
//            }

//            return theta;
//        }


//        public Matrix<float> filter_invKin(Matrix<float> theta, Matrix<float> pose)
//        {
//            int r = theta.RowCount;
//            int c = theta.ColumnCount;
//            Matrix<float> newTheta = Matrix<float>.Build.Dense(r, 1, float.NaN);

//            for (int i = 0; i < c; i++)
//            {
//                Vector<float> col = theta.Column(i);
//                if (col.Exists((float f) => { return f == float.NaN; }))
//                { // NaN exists, do not keep
//                    Debug.Log("NaN found, skipped solution");
//                }
//                else
//                { // More checks
//                    Matrix<float> newPose = fwd_kin(col);
//                    Debug.Log("New Robot Pose: " + newPose.ToString(4, 4));
//                    // note: coords[3] should always be 1, so it should be fine to leave it?
//                    var coords = newPose.Column(3);
//                    var oldCoords = pose.Column(3);
//                    float d = (float)(coords - oldCoords).L2Norm();
//                    Debug.Log("Distance between EE coords: " + d);

//                    //float x = newPose[0, 3];
//                    //float y = newPose[1, 3];
//                    float z = coords[2];

//                    // Check EE height AND distance between soln & desired loc
//                    if (z > 0.1 && d < 0.1) {

//                        // Check joint limits
//                        bool violated = false;
//                        for (int j = 0; j < 6; j++)
//                        {
//                            if (col[j] > upperLimits[j] || col[j] < lowerLimits[j])
//                                violated = true;
//                        }

//                        if (!violated) {
//                            //Debug.Log("Adding vector:" + theta.SubMatrix(0, r, i, 1));
//                            newTheta = newTheta.Append(theta.SubMatrix(0, r, i, 1));
//                        }
//                        else {
//                            Debug.Log("Did not add vector (joint limits):" + theta.SubMatrix(0, r, i, 1));
//                        }

//                    }
//                    else
//                        Debug.Log("Did not add vector (zcoord):" + theta.SubMatrix(0, r, i, 1));
//                }
//            }
//            if (newTheta.ColumnCount > 1)
//            {
//                newTheta = newTheta.RemoveColumn(0);  // remove placeholder
//            }

//            // Check 'distances' & sort results by lowest
//            newTheta = sortResults(newTheta);

//            return newTheta;
//        }


//        private Matrix<float> sortResults(Matrix<float> newTheta) {
//            int r = newTheta.RowCount;  // Should always be 6
//            int c = newTheta.ColumnCount;

//            Vector<float> jointDistances = Vector<float>.Build.Dense(r);
//            Vector<float> overallDistances = Vector<float>.Build.Dense(c);

//            for (int i = 0; i < c; i++)
//            {
//                jointDistances = newTheta.Column(i) - prevJoints;
//                //Debug.Log("Joint distances for column " + i + " :" + jointDistances);
//                overallDistances[i] = (float)jointDistances.L2Norm();  // sqrt of sum of sqrs
//            }
//            Debug.Log("Overall distances: " + overallDistances);

//            // Sort indices by lowest distance
//            Vector<float> indices = Vector<float>.Build.Dense(c, i => i);
//            MathNet.Numerics.Sorting.Sort<float, float>(overallDistances, indices);
//            Debug.Log("Sorted indices: " + indices);

//            Matrix<float> sortedTheta = Matrix<float>.Build.Dense(r, c);
//            for (int i = 0; i < c; i++) {
//                sortedTheta.SetColumn(i, newTheta.Column((int)indices[i]) );
//            }
//            //Debug.Log("Sorted solutions: " + sortedTheta);

//            return sortedTheta;
//        }

//        public Matrix<float> filter_invKin_OLD(Matrix<float> theta)
//        {
//            Matrix<float> newTheta = Matrix<float>.Build.Dense(6, 1, 0f);

//            for (int i = 0; i < theta.ColumnCount; i++)
//            {
//                if (theta[1, i] < 0 && theta[1, i] >= (-1 * Mathf.PI / 2) && theta[2, i] > 0)
//                { // keep
//                    newTheta.Append(theta.SubMatrix(0, 6, i, 1));
//                }
//                else if (theta[1, i] < (-1 * Mathf.PI / 2) && theta[1, i] >= (-1 * Mathf.PI) && theta[2, i] < 0)
//                { // keep
//                    newTheta.Append(theta.SubMatrix(0, 6, i, 1));
//                }
//                else
//                { // don't keep
//                }
//            }
//            if (newTheta.ColumnCount > 1)
//            {
//                newTheta.RemoveColumn(0);  // remove placeholder
//            }
//            return newTheta;
//        }
//    }
//}
