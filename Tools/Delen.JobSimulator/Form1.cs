using System;
using System.IO;
using System.Windows.Forms;
using Delen.Agent.Communication;
using Delen.Agent.Tests.Functional;
using Delen.Core.Communication;
using Delen.Server.Tests.Common;
using Delen.Test.NUnitSample;
using ServerChannel = Delen.Server.Tests.Common.ServerChannel;

namespace Delen.JobSimulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtRunner.Text = @"C:\Program Files (x86)\NUnit-2.6.2\bin\nunit-console-x86.exe"; //@"c:\windows\system32\cmd.exe";
            txtArguments.Text=@"Debug\RedGate.AcceptanceTests.dll /output TestResults.xml";
        }

        private void btnSubmitCommand_Click(object sender, EventArgs e)
        {
            var bytes = File.ReadAllBytes(@"C:\Code\RedGate.BuildTools\AcceptanceTests\bin\Debug.zip");
            var command = new AddJobRequest() { Arguments = txtArguments.Text, InitiatedBy = "Me", Runner = txtRunner.Text,  WorkDirectoryArchive = bytes};
            var uriFactory = this.checkBox1.Checked ? new UriFactoryForTesting(Settings.Default) : new UriFactory(Settings.Default);
            var response=  ServerChannel.SendRequest<CreateEntityResponse<int>>(uriFactory.Create(EndPoint.QueueJob), command);
            MessageBox.Show(@"Job added " + response.EntityIdentifier);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            const string runner = @"C:\Program Files (x86)\NUnit-2.6.2\bin\nunit-console-x86.exe";
            string arguments = string.Format(@"{0} ", BaseFixture.AssemblyName  + ".dll");

            var workDirectoryArchive =  WorkFixture.CreateWorkDirectoryArchive(@"..\..\..\..\..\Tests\Delen.Test.NUnitSample\bin\Debug\");
            var command = new AddJobRequest
            {
                Arguments = arguments,
                InitiatedBy = "Me",
                Runner = runner,
                WorkDirectoryArchive = File.ReadAllBytes(workDirectoryArchive),
                ArtifactSearchFilter = "TestResult.xml"
            };
             
                ServerChannel.SendRequest<CreateEntityResponse<int>>(
                    new UriFactory(Settings.Default).Create(EndPoint.QueueJob), command);
            MessageBox.Show(@"Done.");
            
        }

       
    }
}
