using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVGB07_Modul3
{
    public partial class TextEditorForm : Form
    {
        private string  fileName;
        private string  filePath;
        private string  fileText;
        private bool    fileChanged;

        public TextEditorForm()
        {
            //Default file name
            fileName = "untitled.txt";
            
            Text = fileName;
            fileChanged = false;
            
            InitializeComponent();
        }

        ////======================= Component functions =======================
        //New file-button
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check if file has been altered without saving
            if (!fileChanged)
                //Has not been altered without saving
                newFile();
            else
            {
                //Has been altered without saving
                //Run exit function
                var result = exitMessageBox();

                //If user wants to save before exiting
                if (result == DialogResult.Yes)
                {
                    //If file has existing path (if file has been saved before)
                    if (string.IsNullOrEmpty(filePath))
                    {
                        saveFileAs();
                    }
                    else
                    {
                        saveFile();
                    }
                    newFile();
                }
                //If user do not want to save before exiting
                else if (result == DialogResult.No)
                {
                    newFile();
                }
                //If user pressed cancel do nothing
            }
        }

        //Open file-button
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check if file has been altered without saving
            if (!fileChanged)
            {
                //Has not been altered without saving
                openFile();
            }
            else
            {
                //Has been altered without saving
                //Ask if user wants to save before exiting
                var result = exitMessageBox();

                switch (result)
                {
                    //If user wants to save before exiting
                    case DialogResult.Yes:
                        //Checks if file has existing path (if file has been saved before)
                        if (string.IsNullOrEmpty(filePath)) {   saveFileAs();  }
                        else {  saveFile(); }
                        openFile();
                        break;

                    //If user do not want to save before exiting
                    case DialogResult.No:
                        openFile();
                        break;

                    //If user pressed cancel do nothing
                    case DialogResult.Cancel:
                        break;
                    default:
                        break;
                }
            }
        }

        //SaveFile-Button
        private void saveItem_Click(object sender, EventArgs e)
        {
            //If file has not been saved or if user press Save as-button
            var btn = (ToolStripMenuItem)sender;

            if (string.IsNullOrEmpty(filePath) || btn.AccessibleName == "SaveAs")
                saveFileAs();
            else
                saveFile();
        }

        //TextBoxChanged-function, triggers every time the textbox changes value
        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            hasChanged();
            fileText = richTextBox.Text;

            //Run and calculate all statusbar information
            setCharIncSpace();
            setCharExcSpace();
            setWords();
            setRows();
        }
        
        //Exit-button, closes the program by calling TextEditorForm_FormClosing-function
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Exit-function, gets called when user wants to exit the program
        private void TextEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (fileChanged)
            {
                var result = exitMessageBox();

                switch (result)
                {
                    case DialogResult.Yes:
                        saveFileAs();
                        if (fileChanged)
                            e.Cancel = true;
                        break;
                    case DialogResult.No:
                        //this.Close();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    default:
                        break;
                }
            }
        }
        
        //======================= "Backend" functions =======================
        //NewFile-function, gets called to clear the form
        private void newFile()
        {
            richTextBox.Clear();
            fileName = "untitled.txt";
            filePath = string.Empty;
            Text = fileName;
            fileChanged = false;
        }

        //Open file-function, loads file in to the program
        private void openFile()
        {
            //Open OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Textdocument|*.txt";

            //If user pressed open in the OpenFileDialog
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Set the file information in the program
                filePath = openFileDialog.FileName;
                fileName = openFileDialog.SafeFileName;
                fileText = File.ReadAllText(filePath);

                Text = fileName;
                richTextBox.Text = fileText;

                //filling the textbox with the filetext will activate "TextBox_TextChanged" function, which triggers the "hasChanged" function
                //We need to nullify "hasChanged" settings since no real change has been made
                Text = fileName;
                fileChanged = false;

                //Sets the curson at the end of the text
                richTextBox.Focus();
                richTextBox.SelectionStart = richTextBox.Text.Length;
            }
        }

        //SaveFileAs-function, opens saveFileDialog and calls saveFile-function
        private void saveFileAs()
        {
            //Open savefiledialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Textdocument|*.txt";

            //If user pressed save in the SaveFileDialog
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = saveFileDialog.FileName;
                fileName = Path.GetFileName(saveFileDialog.FileName);
                saveFile();
            }
        }

        //SaveFile-function, writes file to disk
        private void saveFile()
        {
            try
            {
                File.WriteAllText(filePath, fileText);
                Text = fileName;
                fileChanged = false;
                MessageBox.Show("File saved successfully!");
            }
            catch (System.ArgumentException e)
            {
                MessageBox.Show($"Could not save file.\n{e.Message}");
            }
        }

        //File has changed-function, gets called when file has been changed
        private void hasChanged()
        {
            //If file has been altered, put a '*' before file name in the title
            Text = "*" + fileName;
            fileChanged = true;
        }

        //ExitMessageBox-function, gets called when user wants to exit with unsaved changes
        private DialogResult exitMessageBox()
        {
            //Show MessageBox with buttons "yes, no, cancel" and returns the result
            var option = MessageBox.Show(
                "You have unsaved changes, do you want to save before exiting?", 
                "Unsaved changes", 
                MessageBoxButtons.YesNoCancel, 
                MessageBoxIcon.Warning
                );
            return option;
        }

        //======================= Statusbar functions =======================
        //Calculate characters including spaces-function 
        private void setCharIncSpace()
        {
            //Remove all enter and return by replacing them with nothing, then calculate all the characters including spaces in the string
            string tempString = fileText.Replace("\n", "").Replace("\r", "");
            charIncSpaceLabel.Text = tempString.Count().ToString();
        }

        //Calculate characters excluding spaces-function
        private void setCharExcSpace()
        {
            //Remove all enter, return and spaces by replacing them with nothing, then calculate all the characters in the string
            string tempString = fileText.Replace("\n", "").Replace("\r", "").Replace(" ", "");
            charExcSpaceLabel.Text = tempString.Count().ToString();
        }

        //Calculate words-function
        private void setWords()
        {
            bool indexInWord = false;
            int words = 0;

            //Remove all spaces before and after the first and last character in the string
            string tempString = fileText.Trim();
            //Remove all enter, return and spaces by replacing them with nothing
            tempString = tempString.Replace("\n", " ").Replace("\r", " ");

            //Iterate through the string
            for (int i = 0; i < tempString.Length; i++)
            {   
               //If character is a space, then we are not in a word. Set indexInWord to false
               if (char.IsWhiteSpace(tempString[i]))
               {
                    indexInWord = false;
               }
               //else character is not a space which means we are in a word
               else
               {    
                    //If indexInWord is false it means we just went from a space-sign to a character sign. We just entered a word
                    if (!indexInWord)
                    {
                        //increment words-variable by 1 and set indexInWord to true while we are iterating though the word
                        words++;
                        indexInWord = true;
                    }
               }
            }
            wordsLabel.Text = words.ToString();
        }

        //Calculate words-function
        private void setRows()
        {
            int rows = 1;
            //Iterate through the string and count the '\n'
            foreach (var c in fileText)
            {
                if (c == '\n')
                    rows++;
            }
            //If we have deleted all the text in our textbox we set variabel to 0 (would otherwise be 1)
            if (string.IsNullOrEmpty(fileText))
                rowsLabel.Text = "0";
            else
                rowsLabel.Text = rows.ToString();
        }

        private void TextEditorForm_Load(object sender, EventArgs e)
        {
            richTextBox.AllowDrop = true;
            richTextBox.DragDrop += RichTextBox_DragDrop;
        }

        private void RichTextBox_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var fileNames = data as string[];
                if (fileNames.Length > 0)
                    if (e.KeyState == 4)
                    {
                        var index = richTextBox.SelectionStart;
                        richTextBox.Text = richTextBox.Text.Substring(0, index) + File.ReadAllText(fileNames[0]) + richTextBox.Text.Substring(index);
                        MessageBox.Show("Key that was pressed is shift");
                    }
                    else if (e.KeyState == 8)
                    {
                        richTextBox.Text = fileText + File.ReadAllText(fileNames[0]);
                        MessageBox.Show("Key that was pressed is control");
                    }
                    else if(e.KeyState == 0)
                    {
                        if (fileChanged)
                        {
                            var result = exitMessageBox();

                            switch (result)
                            {
                            case DialogResult.Yes:
                                //Checks if file has existing path (if file has been saved before)
                                if (string.IsNullOrEmpty(filePath)) {   saveFileAs();  }
                                else {  saveFile(); }
                                richTextBox.Text = File.ReadAllText(fileNames[0]);
                                break;

                            //If user do not want to save before exiting
                            case DialogResult.No:
                                richTextBox.Text = File.ReadAllText(fileNames[0]);
                                break;

                            //If user pressed cancel do nothing
                            case DialogResult.Cancel:
                                break;
                            default:
                                break;
                            }
                        }
                        else
                        {
                            richTextBox.Text = File.ReadAllText(fileNames[0]);
                        }
                    }
            }
        }
    }
}
