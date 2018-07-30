///////////////////////////////////////////////////////////////////////////////
//
//  Original System: UR5_kinematics.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Manus_interpreter.cs
//  Revision:        1.0 7/12/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  IK programming for virtual UR5.
//
//  get_vector_UR5() - Returns a joint configuration present for the UR5 
//
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
//using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;

public class ur5_kinematics : MonoBehaviour
{
    public Button thisButton;
    public InputField outputText;
    public float METER_RATIO;
    public Button testButton;
    public Button goButton;
    public GameObject locationSphere;
    public bool enabled = true;
    public bool debug_pose = false;
    public float[] output_xyz_rot = new float[6];
    public float[] angle_vector = new float[6];


    //[Range(0, 360)]
    //public int offset;


    //Source 0 is the only one that best matches
    [Range(0, 7)]
    public int select_source;

    public float offset_0 = -42.18f, offset_1, offset_2, offset_3, offset_4, offset_5;
    private ur5 robotModel;

    public UR5Controller controller;
    public GetFromServer gfs;
    private GameObject[] jointList = new GameObject[6];
    private Slider[] sliderList = new Slider[6];


    Matrix<float> robotTheta_filt;

    // Use this for initialization
    void Start()
    {
        //thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(TaskOnClick);

        testButton.onClick.AddListener(TaskOnClick2);
        //goButton.onClick.AddListener(TaskOnClick3);

        robotModel = new ur5();

        controller.initializeJoints(jointList);
        controller.initializeSliders(sliderList);

        outputText.text = "0.0  0.0  0.0  0.0  0.0  0.0  ";

    }

    //Update is called once per frame

    //void Update()
    //{
    //    return;
    //}


    //  Called when get-IK button is clicked
    void TaskOnClick()
    {
        // Get OFFSET values from sliders and run FK to get pose
        Vector<float> jointVals = Vector<float>.Build.DenseOfArray(controller.offsetSliderValues(controller.getSliderList()));
        Matrix<float> robotPose = robotModel.fwd_kin(jointVals.Multiply((Mathf.PI / 180f)));  // in rads
        Debug.Log("Input Robot Pose: " + robotPose.ToString(4, 4));
        robotModel.setCurrentJoints(jointVals.Multiply((Mathf.PI / 180f)));

        // Use pose to run IK
        Matrix<float> robotTheta = robotModel.inv_kin(robotPose);
        //Debug.Log("Theta from IK: " + robotTheta.ToString(6, 8));
        Debug.Log("Theta from IK: " + robotTheta.Multiply((180f / Mathf.PI)).ToString(6, 8));

        // Filter results using EE pose and joint limits
        robotModel.setLimits(controller.upperLimit_r, controller.lowerLimit_r);
        robotTheta_filt = robotModel.filter_invKin(robotTheta, robotPose);
        //Debug.Log("Filtered theta: " + robotTheta_filt.ToString(6, 8));
        Debug.Log("Filtered theta: " + robotTheta_filt.Multiply((180f / Mathf.PI)).ToString(6, 8));

        // Display in text field
        outputText.text = robotTheta_filt.Column(0).Multiply((180f / Mathf.PI)).ToVectorString("0.0  ");
    }


    //  Called when go-to-IK TEST button is clicked
    void TaskOnClick2()
    {
        var soln = robotTheta_filt.Column(0).Multiply((180f / Mathf.PI));
        Debug.Log(soln);
        controller.setSliderList(controller.offsetJointValues(soln.ToArray()));
    }


    //  Called when go-to-IK button (SPHERE) is clicked
    //void TaskOnClick3()

    void Update()
    {
        /*The ever-more mysterious IK method. Use with caution. Method confirmed to only work with UR5 model located at origin with some offset to base.
         * Offset set in the output value for the robot pose is required, otherwise weird behaviour is expected. 
         */
        if (enabled)
        {
            float x, y, z, rx, ry, rz;
            //Rotation matix
            Matrix<float> world_to_robot = robotModel.format_pose(0, 0, 0, 0, 0, 0);
            Matrix<float> robot_to_world = world_to_robot.Transpose();

            //My position of handling point
            Vector3 pt = locationSphere.transform.position;
            Vector3 eRot = locationSphere.transform.eulerAngles;
            eRot = eRot * Mathf.PI / 180F;

            //Setting up variables
            pt = pt * METER_RATIO;
            x = -pt.x;
            y = pt.y;
            z = pt.z;
            rx = eRot.x;
            ry = eRot.y;
            rz = eRot.z;

            //Format the position into a matrix
            Matrix<float> robot_to_cube = robotModel.format_pose(x, y, z, rx, ry, rz);
            Matrix<float> world_to_cube = robot_to_cube.Multiply(robot_to_world);
            Matrix<float> matrix_thetha = robotModel.inv_kin(world_to_cube);
            Vector<float> temp_sol = matrix_thetha.Column(select_source);

            //Debug Only
            //Debug.Log("Sanity");
            //Debug.Log(matrix_thetha);
            //Debug.Log(world_to_cube);

            /*  Data formatter for matrix*/
            //Matrix<float> fwd_kin_mat = robotModel.fwd_kin(temp_sol);
            //locate_real_world_pt(fwd_kin_mat);


            //Debug.Log(sanity_check);

            //Convert into degrees
            var soln = matrix_thetha.Column(select_source).Multiply((180f / Mathf.PI));
            //Debug.Log(soln);
            //Conditional for debug pose
            if (debug_pose)
            {
                float[] my_vals = { -42.58f, -43.69f, -99.57f, 233.2f, -89.66f, -47.09f };
                angle_vector = my_vals;
                controller.setSliderList(controller.offsetJointValues(my_vals));
            }
            else
            {
                float[] array_sol = soln.ToArray();
                array_sol[0] += offset_0; //Offset set to -42.18
                array_sol[1] += offset_1;
                array_sol[2] += offset_2;
                array_sol[3] += offset_3;
                array_sol[4] += offset_4;
                array_sol[5] += offset_5;
                angle_vector = array_sol;
                controller.setSliderList(controller.offsetJointValues(array_sol));

            }

            outputText.text = matrix_thetha.Column(0).Multiply((180f / Mathf.PI)).ToVectorString("0.0  ");
        }
    }

    void locate_real_world_pt(Matrix<float> fwd_kin_mat)
    {
        //Get the Rotational Matrix from the fwd_kinematics
        Matrix<float> rot_mat = Matrix<float>.Build.DenseOfArray(new float[,] {
                { fwd_kin_mat[0,0],fwd_kin_mat[0,1],fwd_kin_mat[0,2] },
                { fwd_kin_mat[1,0],fwd_kin_mat[1,1],fwd_kin_mat[1,2] },
                { fwd_kin_mat[2,0],fwd_kin_mat[2,1],fwd_kin_mat[2,2] } });

        //Rotate on the Z axis by 42.18 degrees to match box
        Matrix<float> z_rot = Matrix<float>.Build.DenseOfArray(new float[,] {
                {0.7410F,-0.6714F,0 },
                {0.6714F, 0.7410F,0 },
                {0,0,1 },           });

        //Multiply and Negate the X axis
        Matrix<float> final_mat = rot_mat.Multiply(z_rot);
        final_mat[0, 0] = final_mat[0, 0] * -1;
        final_mat[1, 0] = final_mat[1, 0] * -1;
        final_mat[2, 0] = final_mat[2, 0] * -1;
        
        //Get the Euler Angles
        Vector<float> rotations = robotModel.euler_from_mat(final_mat, "xyz");

        //XYZ position fix
        Vector<float> positions = Vector<float>.Build.Dense(new float[] { fwd_kin_mat[0,3], fwd_kin_mat[ 1,3], fwd_kin_mat[2,3] }); //ZXY => XYZ
        positions = positions.Multiply(9.82216F);   //=> Convert to "real" units
        positions = positions.Multiply(1000F);       //=> Convert to milimmeters 
        
        //Set global array
        set_final_pos_UR5(positions, rotations);
    }

    void set_final_pos_UR5(Vector<float> position, Vector<float> euler_form)
    {
        List<float> temp_list = new List<float>();
        foreach (float i in position)
        {
            temp_list.Add(i);
        }
        foreach (float j in euler_form)
        {
            temp_list.Add(j);
        }
        output_xyz_rot = temp_list.ToArray();
    }

    public float[] get_vector_UR5()
    {
        //return output_xyz_rot;
        return angle_vector;

    }





    //  Converted python code from SK Gupta's lab
    public class ur5
    {
        private Vector<float> a = Vector<float>.Build.Dense(6);
        private Vector<float> d = Vector<float>.Build.Dense(6);
        private Vector<float> alpha = Vector<float>.Build.Dense(6);
        private Matrix<float> dh = Matrix<float>.Build.Dense(4, 4);

        private Vector<float> upperLimits = Vector<float>.Build.Dense(6);
        private Vector<float> lowerLimits = Vector<float>.Build.Dense(6);
        private Vector<float> prevJoints = Vector<float>.Build.Dense(6);


        public ur5()
        {
            // a:  length offset between joints (e.g. length of arm segments in m)
            a.SetValues(new float[] { 0, -0.425f, -0.39225f, 0, 0, 0 });
            //a.SetValues(new float[] { 0, -0.425f, -0.39225f, 0, 0, 0 });
            d.SetValues(new float[] { 0.089159f, 0, 0, 0.10915f, 0.09465f, 0.0823f });
            alpha.SetValues(new float[] { Mathf.PI / 2, 0, 0, Mathf.PI / 2, -1 * Mathf.PI / 2, 0 });
        }


        public void setLimits(float[] upper, float[] lower)
        {
            upperLimits = Vector<float>.Build.DenseOfArray(upper);
            lowerLimits = Vector<float>.Build.DenseOfArray(lower);
            // Convert to rads
            upperLimits = upperLimits.Multiply((Mathf.PI / 180f));
            lowerLimits = lowerLimits.Multiply((Mathf.PI / 180f));
        }

        public void setCurrentJoints(Vector<float> joints)
        {
            prevJoints = joints;
        }



        private Matrix<float> DH(float aa, float aalpha, float dd, float ttheta)
        {
            dh = Matrix<float>.Build.DenseOfArray(new float[,] {
                { Mathf.Cos(ttheta), -1 * Mathf.Sin(ttheta) * Mathf.Cos(aalpha), Mathf.Sin(ttheta) * Mathf.Sin(aalpha), aa* Mathf.Cos(ttheta) },
                { Mathf.Sin(ttheta), Mathf.Cos(ttheta) * Mathf.Cos(aalpha), -1 * Mathf.Cos(ttheta) * Mathf.Sin(aalpha), aa* Mathf.Sin(ttheta) },
                { 0, Mathf.Sin(aalpha), Mathf.Cos(aalpha), dd},
                { 0, 0, 0, 1} });


            for (int i = 0; i < dh.RowCount; i++)
            {
                for (int j = 0; j < dh.ColumnCount; j++)
                {
                    if (Mathf.Abs(dh[i, j]) < 0.0001f)
                        dh[i, j] = 0.0f;
                }
            }

            return dh;
        }

        // in rads!
        public Matrix<float> fwd_kin(Vector<float> joints)
        {
            var T01 = DH(a[0], alpha[0], d[0], joints[0]);
            var T12 = DH(a[1], alpha[1], d[1], joints[1]);
            var T23 = DH(a[2], alpha[2], d[2], joints[2]);
            var T34 = DH(a[3], alpha[3], d[3], joints[3]);
            var T45 = DH(a[4], alpha[4], d[4], joints[4]);
            var T56 = DH(a[5], alpha[5], d[5], joints[5]);

            //return Matrix<float>.op_DotMultiply(np.dot(np.dot(np.dot(np.dot(T01, T12), T23), T34), T45), T56);
            return (((((T01 * T12) * T23) * T34) * T45) * T56);
        }

        public Vector<float> euler_from_mat(Matrix<float> rotation_matrix, string euler_type)
        {
            //% gamma is rotation about x
            //%beta is rotation about y
            //%alpha is rotation about z
            //% returns Euler ZYX angles from rotation matrix
             
            float alpha = 0, gamma = 0, beta = 0;
            float r11 = rotation_matrix[0, 0];
            float r12 = rotation_matrix[0, 1];
            float r13 = rotation_matrix[0, 2];
            float r21 = rotation_matrix[1, 0];
            float r22 = rotation_matrix[1, 1];
            float r23 = rotation_matrix[1, 2];
            float r31 = rotation_matrix[2, 0];
            float r32 = rotation_matrix[2, 1];
            float r33 = rotation_matrix[2, 2];

            //% alpha is angle about z
            // % beta is angle about y
            //  % gamma is angle about x
            if (euler_type == "zyx")
            {
                //% gimbal
                if (r11 == 0 && r21 == 0)
                {
                    alpha = 0;
                    beta = Mathf.PI / 2;
                    gamma = Mathf.Atan2(r12, r22);
                }
                else
                {
                    alpha = Mathf.Atan2(r21, r11);
                    beta = Mathf.Atan2(-r31, Mathf.Sqrt((r11 * r11) + (r21 * r21)));
                    gamma = Mathf.Atan2(r32, r33);
                }
            }
            else if (euler_type == "xyz")
            {
                alpha = Mathf.Atan2(-r12, r11);
                gamma = Mathf.Atan2(-r23, r33);
                beta = Mathf.Atan2(r13, r33 / Mathf.Cos(gamma));
            }
            float[] Euler = { gamma, beta, alpha };
            Vector<float> yaw_pitch_roll = Vector<float>.Build.Dense(Euler);

            return yaw_pitch_roll.Multiply(180 / Mathf.PI);
        }



        //Formatted in Positional(Meters) and rotational(Radians) 
        public Matrix<float> format_pose(float x_pos, float y_pos, float z_pos, float x_rot, float y_rot, float z_rot)
        {
            Matrix<float> pose = Matrix<float>.Build.Dense(4, 4);

            // roll, pitch, yaw -> z, y, x
            float roll = z_rot;
            float pitch = y_rot;
            float yaw = x_rot;

            pose[0, 0] = Mathf.Cos(pitch) * Mathf.Cos(roll);
            pose[0, 1] = Mathf.Cos(yaw) * Mathf.Sin(roll) + Mathf.Cos(roll) * Mathf.Sin(pitch) * Mathf.Sin(yaw);
            pose[0, 2] = Mathf.Sin(roll) * Mathf.Sin(yaw) - Mathf.Cos(roll) * Mathf.Cos(yaw) * Mathf.Sin(pitch);

            pose[1, 0] = -1f * Mathf.Cos(pitch) * Mathf.Sin(roll);
            pose[1, 1] = Mathf.Cos(roll) * Mathf.Cos(yaw) - Mathf.Sin(pitch) * Mathf.Sin(roll) * Mathf.Sin(yaw);
            pose[1, 2] = Mathf.Cos(roll) * Mathf.Sin(yaw) + Mathf.Cos(yaw) * Mathf.Sin(pitch) * Mathf.Sin(roll);

            pose[2, 0] = Mathf.Sin(pitch);
            pose[2, 1] = -Mathf.Cos(pitch) * Mathf.Sin(yaw);
            pose[2, 2] = Mathf.Cos(pitch) * Mathf.Cos(yaw);

            pose[0, 3] = z_pos;
            pose[1, 3] = x_pos;
            pose[2, 3] = y_pos;
            pose[3, 3] = 1f;

            return pose;
        }



        public Matrix<float> inv_kin(Matrix<float> pose)
        {
            // pose is the 4x4 matrix of the end effector

            Matrix<float> theta = Matrix<float>.Build.Dense(6, 8);

            // theta1
            var temp1 = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, -d[5], 1 });
            var temp2 = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 1 });
            var p05 = (pose * temp1) - temp2;
            var psi = Mathf.Atan2(p05[1], p05[0]);
            var phi = 0f;
            if (d[3] / Mathf.Sqrt(p05[1] * p05[1] + p05[0] * p05[0]) > 1)
                phi = 0f;
            else
                phi = Mathf.Acos(d[3] / Mathf.Sqrt(p05[1] * p05[1] + p05[0] * p05[0]));
            //theta[0, :4] = m.pi / 2 + psi + phi;
            theta.SetSubMatrix(0, 0, Matrix<float>.Build.Dense(1, 4, Mathf.PI / 2 + psi + phi));
            //theta[0, 4:8] = Mathf.PI / 2 + psi - phi;
            theta.SetSubMatrix(0, 4, Matrix<float>.Build.Dense(1, 4, Mathf.PI / 2 + psi - phi));

            // theta5
            for (int c = 0; c < 4; c++)
            {
                var T10 = DH(a[0], alpha[0], d[0], theta[0, c]).Inverse();
                var T16 = (T10 * pose);
                var p16z = T16[2, 3];
                var t5 = 0f;
                if ((p16z - d[3]) / d[5] > 1)
                    t5 = 0f;
                else
                    t5 = Mathf.Acos((p16z - d[3]) / d[5]);

                //theta[4, c: c + 1 + 1] = t5;
                //theta[4, c + 2:c + 3 + 1] = -t5;
                theta[4, c] = t5;
                theta[4, c + 1] = t5;
                theta[4, c + 2] = -t5;
                theta[4, c + 3] = -t5;
            }

            // theta6
            for (int c = 0; c <= 6 && c % 2 == 0; c++)
            {
                var T01 = DH(a[0], alpha[0], d[0], theta[0, c]);
                var T61 = pose.Inverse() * T01;
                var T61zy = T61[1, 2];
                var T61zx = T61[0, 2];
                var t5 = theta[4, c];
                //theta[5, c: c + 1 + 1] = Mathf.Atan2(-T61zy / Mathf.Sin(t5), T61zx / Mathf.Sin(t5));
                theta[5, c] = Mathf.Atan2(-T61zy / Mathf.Sin(t5), T61zx / Mathf.Sin(t5));
                theta[5, c + 1] = Mathf.Atan2(-T61zy / Mathf.Sin(t5), T61zx / Mathf.Sin(t5));
            }

            // theta3
            for (int c = 0; c <= 6 && c % 2 == 0; c++)
            {
                var T10 = DH(a[0], alpha[0], d[0], theta[0, c]).Inverse();
                var T65 = DH(a[5], alpha[5], d[5], theta[5, c]).Inverse();
                var T54 = DH(a[4], alpha[4], d[4], theta[4, c]).Inverse();
                var T14 = ((T10 * pose) * (T65 * T54));
                temp1 = Vector<float>.Build.DenseOfArray(new float[] { 0, -d[3], 0, 1 });
                temp2 = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 1 });
                var p13 = (T14 * temp1) - temp2;
                //var p13norm2 = la.norm(p13) * *2;  // Frobenius norm
                var p13norm2 = p13.Norm(2) * p13.Norm(2);  // L2 norm is same?
                var t3p = 0f;
                if ((p13norm2 - a[1] * a[1] - a[2] * a[2]) / (2 * a[1] * a[2]) > 1)
                    t3p = 0f;
                else
                    t3p = Mathf.Acos(((float)p13norm2 - a[1] * a[1] - a[2] * a[2]) / (2 * a[1] * a[2]));
                theta[2, c] = t3p;
                theta[2, c + 1] = -t3p;
            }

            // theta2 theta4
            for (int c = 0; c < 8; c++)
            {
                var T10 = DH(a[0], alpha[0], d[0], theta[0, c]).Inverse();
                var T65 = DH(a[5], alpha[5], d[5], theta[5, c]).Inverse();
                var T54 = DH(a[4], alpha[4], d[4], theta[4, c]).Inverse();
                var T14 = ((T10 * pose) * (T65 * T54));
                temp1 = Vector<float>.Build.DenseOfArray(new float[] { 0, -d[3], 0, 1 });
                temp2 = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 1 });
                var p13 = (T14 * temp1) - temp2;
                var p13norm = p13.Norm(2);
                theta[1, c] = -Mathf.Atan2(p13[1], -p13[0]) + Mathf.Asin(a[2] * Mathf.Sin(theta[2, c]) / (float)p13norm);
                var T32 = DH(a[2], alpha[2], d[2], theta[2, c]).Inverse();
                var T21 = DH(a[1], alpha[1], d[1], theta[1, c]).Inverse();
                var T34 = ((T32 * T21) * T14);
                theta[3, c] = Mathf.Atan2(T34[1, 0], T34[0, 0]);
            }

            for (int i = 0; i < theta.RowCount; i++)
            {
                for (int j = 0; j < theta.ColumnCount; j++)
                {
                    if (theta[i, j] > Mathf.PI)
                        theta[i, j] = theta[i, j] - 2 * Mathf.PI;
                    if (theta[i, j] < -1 * Mathf.PI)
                        theta[i, j] = theta[i, j] + 2 * Mathf.PI;
                }
            }

            return theta;
        }


        public Matrix<float> filter_invKin(Matrix<float> theta, Matrix<float> pose)
        {
            int r = theta.RowCount;
            int c = theta.ColumnCount;
            Matrix<float> newTheta = Matrix<float>.Build.Dense(r, 1, float.NaN);

            for (int i = 0; i < c; i++)
            {
                Vector<float> col = theta.Column(i);
                Matrix<float> NaN_chk = fwd_kin(col);
                if (col.Exists((float f) => { return f == float.NaN; }))
                { // NaN exists, do not keep
                    Debug.Log("NaN found, skipped solution");
                }
                else
                { // More checks
                    Matrix<float> newPose = fwd_kin(col);
                    Debug.Log("New Robot Pose: " + newPose.ToString(4, 4));
                    // note: coords[3] should always be 1, so it should be fine to leave it?
                    var coords = newPose.Column(3);
                    var oldCoords = pose.Column(3);
                    float d = (float)(coords - oldCoords).L2Norm();
                    Debug.Log("Distance between EE coords: " + d);

                    //float x = newPose[0, 3];
                    //float y = newPose[1, 3];
                    float z = coords[2];

                    // Check EE height AND distance between soln & desired loc
                    if (z > 0.1 && d < 0.1)
                    {

                        // Check joint limits
                        bool violated = false;
                        for (int j = 0; j < 6; j++)
                        {
                            if (col[j] > upperLimits[j] || col[j] < lowerLimits[j])
                                violated = true;
                        }

                        if (!violated)
                        {
                            //Debug.Log("Adding vector:" + theta.SubMatrix(0, r, i, 1));
                            newTheta = newTheta.Append(theta.SubMatrix(0, r, i, 1));
                        }
                        else
                        {
                            Debug.Log("Did not add vector (joint limits):" + theta.SubMatrix(0, r, i, 1));
                        }

                    }
                    else
                        Debug.Log("Did not add vector (zcoord):" + theta.SubMatrix(0, r, i, 1));
                }
            }
            if (newTheta.ColumnCount > 1)
            {
                newTheta = newTheta.RemoveColumn(0);  // remove placeholder
            }

            // Check 'distances' & sort results by lowest
            newTheta = sortResults(newTheta);

            return newTheta;
        }


        private Matrix<float> sortResults(Matrix<float> newTheta)
        {
            int r = newTheta.RowCount;  // Should always be 6
            int c = newTheta.ColumnCount;

            Vector<float> jointDistances = Vector<float>.Build.Dense(r);
            Vector<float> overallDistances = Vector<float>.Build.Dense(c);

            for (int i = 0; i < c; i++)
            {
                jointDistances = newTheta.Column(i) - prevJoints;
                //Debug.Log("Joint distances for column " + i + " :" + jointDistances);
                overallDistances[i] = (float)jointDistances.L2Norm();  // sqrt of sum of sqrs
            }
            Debug.Log("Overall distances: " + overallDistances);

            // Sort indices by lowest distance
            Vector<float> indices = Vector<float>.Build.Dense(c, i => i);
            MathNet.Numerics.Sorting.Sort<float, float>(overallDistances, indices);
            Debug.Log("Sorted indices: " + indices);

            Matrix<float> sortedTheta = Matrix<float>.Build.Dense(r, c);
            for (int i = 0; i < c; i++)
            {
                sortedTheta.SetColumn(i, newTheta.Column((int)indices[i]));
            }
            //Debug.Log("Sorted solutions: " + sortedTheta);

            return sortedTheta;
        }

        public Matrix<float> filter_invKin_OLD(Matrix<float> theta)
        {
            Matrix<float> newTheta = Matrix<float>.Build.Dense(6, 1, 0f);

            for (int i = 0; i < theta.ColumnCount; i++)
            {
                if (theta[1, i] < 0 && theta[1, i] >= (-1 * Mathf.PI / 2) && theta[2, i] > 0)
                { // keep
                    newTheta.Append(theta.SubMatrix(0, 6, i, 1));
                }
                else if (theta[1, i] < (-1 * Mathf.PI / 2) && theta[1, i] >= (-1 * Mathf.PI) && theta[2, i] < 0)
                { // keep
                    newTheta.Append(theta.SubMatrix(0, 6, i, 1));
                }
                else
                { // don't keep
                }
            }
            if (newTheta.ColumnCount > 1)
            {
                newTheta.RemoveColumn(0);  // remove placeholder
            }
            return newTheta;
        }
    }


}
