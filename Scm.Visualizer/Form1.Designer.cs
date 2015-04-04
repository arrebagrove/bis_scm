namespace Scm.Visualizer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel = new System.Windows.Forms.Panel();
            this.buttonLoadInstance = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonLoadSolution = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Location = new System.Drawing.Point(0, 41);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(633, 586);
            this.panel.TabIndex = 0;
            this.panel.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelPaint);
            // 
            // buttonLoadInstance
            // 
            this.buttonLoadInstance.Location = new System.Drawing.Point(12, 12);
            this.buttonLoadInstance.Name = "buttonLoadInstance";
            this.buttonLoadInstance.Size = new System.Drawing.Size(109, 23);
            this.buttonLoadInstance.TabIndex = 1;
            this.buttonLoadInstance.Text = "Load Instance";
            this.buttonLoadInstance.UseVisualStyleBackColor = true;
            this.buttonLoadInstance.Click += new System.EventHandler(this.ButtonLoadInstanceClick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.AddExtension = false;
            this.openFileDialog.RestoreDirectory = true;
            // 
            // buttonLoadSolution
            // 
            this.buttonLoadSolution.Location = new System.Drawing.Point(127, 12);
            this.buttonLoadSolution.Name = "buttonLoadSolution";
            this.buttonLoadSolution.Size = new System.Drawing.Size(107, 23);
            this.buttonLoadSolution.TabIndex = 2;
            this.buttonLoadSolution.Text = "Load Solution";
            this.buttonLoadSolution.UseVisualStyleBackColor = true;
            this.buttonLoadSolution.Click += new System.EventHandler(this.ButtonLoadSolutionClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 627);
            this.Controls.Add(this.buttonLoadSolution);
            this.Controls.Add(this.buttonLoadInstance);
            this.Controls.Add(this.panel);
            this.Name = "Form1";
            this.Text = "Visualizer";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Button buttonLoadInstance;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button buttonLoadSolution;

    }
}

