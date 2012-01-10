namespace Viewer.View
{
    partial class OpenGLViewContainer
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.glControl1 = new OpenTK.GLControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonOrtho = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPerspective = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonUndoCamera = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelStatistics = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.glControl1.AutoSize = true;
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(3, 28);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(436, 328);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOrtho,
            this.toolStripButtonPerspective,
            this.toolStripSeparator1,
            this.toolStripButtonUndoCamera,
            this.toolStripSeparator2,
            this.toolStripLabelStatistics});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(442, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonOrtho
            // 
            this.toolStripButtonOrtho.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOrtho.Name = "toolStripButtonOrtho";
            this.toolStripButtonOrtho.Size = new System.Drawing.Size(39, 22);
            this.toolStripButtonOrtho.Text = "Ortho";
            this.toolStripButtonOrtho.Click += new System.EventHandler(this.toolStripButtonOrtho_Click);
            // 
            // toolStripButtonPerspective
            // 
            this.toolStripButtonPerspective.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPerspective.Name = "toolStripButtonPerspective";
            this.toolStripButtonPerspective.Size = new System.Drawing.Size(67, 22);
            this.toolStripButtonPerspective.Text = "Perspective";
            this.toolStripButtonPerspective.Click += new System.EventHandler(this.toolStripButtonPerspective_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonUndoCamera
            // 
            this.toolStripButtonUndoCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndoCamera.Name = "toolStripButtonUndoCamera";
            this.toolStripButtonUndoCamera.Size = new System.Drawing.Size(79, 22);
            this.toolStripButtonUndoCamera.Text = "Reset Camera";
            this.toolStripButtonUndoCamera.Click += new System.EventHandler(this.toolStripButtonUndoCamera_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelStatistics
            // 
            this.toolStripLabelStatistics.Name = "toolStripLabelStatistics";
            this.toolStripLabelStatistics.Size = new System.Drawing.Size(38, 22);
            this.toolStripLabelStatistics.Text = "STATS";
            // 
            // OpenGLViewContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.glControl1);
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "OpenGLViewContainer";
            this.Size = new System.Drawing.Size(442, 367);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonOrtho;
        private System.Windows.Forms.ToolStripButton toolStripButtonPerspective;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndoCamera;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabelStatistics;
    }
}
