using System;
using System.Windows.Forms;

namespace LootScaler
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkQUEST = new System.Windows.Forms.CheckBox();
            this.armorCheck = new System.Windows.Forms.CheckBox();
            this.checkJunk = new System.Windows.Forms.CheckBox();
            this.checkDBC = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.consuCheck = new System.Windows.Forms.CheckBox();
            this.weapCheck = new System.Windows.Forms.CheckBox();
            this.filter = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkUpgrade = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkQUEST);
            this.groupBox1.Controls.Add(this.armorCheck);
            this.groupBox1.Controls.Add(this.checkJunk);
            this.groupBox1.Controls.Add(this.checkDBC);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.consuCheck);
            this.groupBox1.Controls.Add(this.weapCheck);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(292, 168);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FILTERS";
            // 
            // checkQUEST
            // 
            this.checkQUEST.AutoSize = true;
            this.checkQUEST.Location = new System.Drawing.Point(178, 65);
            this.checkQUEST.Name = "checkQUEST";
            this.checkQUEST.Size = new System.Drawing.Size(99, 17);
            this.checkQUEST.TabIndex = 6;
            this.checkQUEST.Text = "QUEST GIVER";
            this.checkQUEST.UseVisualStyleBackColor = true;
            // 
            // armorCheck
            // 
            this.armorCheck.AutoSize = true;
            this.armorCheck.Location = new System.Drawing.Point(6, 42);
            this.armorCheck.Name = "armorCheck";
            this.armorCheck.Size = new System.Drawing.Size(73, 17);
            this.armorCheck.TabIndex = 5;
            this.armorCheck.Text = "ARMORS";
            this.armorCheck.UseVisualStyleBackColor = true;
            // 
            // checkJunk
            // 
            this.checkJunk.AutoSize = true;
            this.checkJunk.Enabled = false;
            this.checkJunk.Location = new System.Drawing.Point(178, 19);
            this.checkJunk.Name = "checkJunk";
            this.checkJunk.Size = new System.Drawing.Size(54, 17);
            this.checkJunk.TabIndex = 4;
            this.checkJunk.Text = "JUNK";
            this.checkJunk.UseVisualStyleBackColor = true;
            // 
            // checkDBC
            // 
            this.checkDBC.AutoSize = true;
            this.checkDBC.Enabled = false;
            this.checkDBC.Location = new System.Drawing.Point(178, 42);
            this.checkDBC.Name = "checkDBC";
            this.checkDBC.Size = new System.Drawing.Size(48, 17);
            this.checkDBC.TabIndex = 3;
            this.checkDBC.Text = "DBC";
            this.checkDBC.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(232, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(52, 46);
            this.button1.TabIndex = 2;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // consuCheck
            // 
            this.consuCheck.AutoSize = true;
            this.consuCheck.Location = new System.Drawing.Point(6, 65);
            this.consuCheck.Name = "consuCheck";
            this.consuCheck.Size = new System.Drawing.Size(107, 17);
            this.consuCheck.TabIndex = 2;
            this.consuCheck.Text = "CONSUMABLES";
            this.consuCheck.UseVisualStyleBackColor = true;
            // 
            // weapCheck
            // 
            this.weapCheck.AutoSize = true;
            this.weapCheck.Location = new System.Drawing.Point(6, 19);
            this.weapCheck.Name = "weapCheck";
            this.weapCheck.Size = new System.Drawing.Size(81, 17);
            this.weapCheck.TabIndex = 1;
            this.weapCheck.Text = "WEAPONS";
            this.weapCheck.UseVisualStyleBackColor = true;
            // 
            // filter
            // 
            this.filter.FormattingEnabled = true;
            this.filter.Location = new System.Drawing.Point(6, 19);
            this.filter.Name = "filter";
            this.filter.Size = new System.Drawing.Size(113, 121);
            this.filter.TabIndex = 7;
            this.filter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.filter_KeyDown);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 142);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(113, 20);
            this.textBox1.TabIndex = 8;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.filter);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Enabled = false;
            this.groupBox3.Location = new System.Drawing.Point(441, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(125, 168);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Entries";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericUpDown1);
            this.groupBox2.Controls.Add(this.checkUpgrade);
            this.groupBox2.Controls.Add(this.checkDBC);
            this.groupBox2.Location = new System.Drawing.Point(12, 176);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(292, 123);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Location = new System.Drawing.Point(104, 40);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(39, 20);
            this.numericUpDown1.TabIndex = 5;
            // 
            // checkUpgrade
            // 
            this.checkUpgrade.AutoSize = true;
            this.checkUpgrade.Location = new System.Drawing.Point(6, 42);
            this.checkUpgrade.Name = "checkUpgrade";
            this.checkUpgrade.Size = new System.Drawing.Size(100, 17);
            this.checkUpgrade.TabIndex = 4;
            this.checkUpgrade.Text = "Bonus Upgrade";
            this.checkUpgrade.UseVisualStyleBackColor = true;
            this.checkUpgrade.CheckedChanged += new System.EventHandler(this.checkUpgrade_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 309);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Burning Crusade - LootScaler";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox consuCheck;
        private System.Windows.Forms.CheckBox weapCheck;
        private System.Windows.Forms.ListBox filter;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private PictureBox pictureBox1;
        private CheckBox checkDBC;
        private CheckBox checkJunk;
        private CheckBox armorCheck;
        private CheckBox checkQUEST;
        private GroupBox groupBox2;
        private CheckBox checkUpgrade;
        private NumericUpDown numericUpDown1;
    }
}

