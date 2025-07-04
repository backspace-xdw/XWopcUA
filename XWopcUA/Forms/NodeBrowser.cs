using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Opc.Ua;
using XWopcUA.Models;
using XWopcUA.Services;
using XWopcUA.Utils;

namespace XWopcUA.Forms
{
    public partial class NodeBrowser : Form
    {
        private readonly OpcUaClient _opcClient;
        private readonly Logger _logger;
        private NodeInfo _selectedNode;

        public NodeInfo SelectedNode => _selectedNode;

        public NodeBrowser(OpcUaClient opcClient)
        {
            InitializeComponent();
            _opcClient = opcClient;
            _logger = Logger.Instance;
            InitializeTreeView();
        }

        private void InitializeTreeView()
        {
            try
            {
                treeNodes.BeginUpdate();
                treeNodes.Nodes.Clear();

                // Add root nodes
                var rootNode = new TreeNode("Server")
                {
                    Tag = new NodeInfo { NodeId = Objects.RootFolder, DisplayName = "Root" }
                };

                // Add standard folders
                AddStandardFolders(rootNode);
                
                treeNodes.Nodes.Add(rootNode);
                rootNode.Expand();
                treeNodes.EndUpdate();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to initialize node browser", ex);
                MessageBox.Show($"Failed to initialize: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddStandardFolders(TreeNode parentNode)
        {
            var standardFolders = new Dictionary<string, NodeId>
            {
                { "Objects", Objects.ObjectsFolder },
                { "Types", Objects.TypesFolder },
                { "Views", Objects.ViewsFolder }
            };

            foreach (var folder in standardFolders)
            {
                var folderNode = new TreeNode(folder.Key)
                {
                    Tag = new NodeInfo 
                    { 
                        NodeId = folder.Value, 
                        DisplayName = folder.Key,
                        IsObject = true
                    }
                };
                
                // Add dummy node to enable expansion
                folderNode.Nodes.Add(new TreeNode("Loading...") { Tag = null });
                parentNode.Nodes.Add(folderNode);
            }
        }

        private void treeNodes_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Tag == null)
            {
                LoadChildNodes(e.Node);
            }
        }

        private void LoadChildNodes(TreeNode parentNode)
        {
            try
            {
                var nodeInfo = parentNode.Tag as NodeInfo;
                if (nodeInfo == null) return;

                parentNode.Nodes.Clear();
                
                var childNodes = _opcClient.Browse(nodeInfo.NodeId);
                
                foreach (var child in childNodes)
                {
                    var childNode = new TreeNode(child.DisplayName)
                    {
                        Tag = child
                    };

                    // Set icon based on node class
                    if (child.IsVariable)
                    {
                        childNode.ImageIndex = 1;
                        childNode.SelectedImageIndex = 1;
                    }
                    else if (child.IsMethod)
                    {
                        childNode.ImageIndex = 2;
                        childNode.SelectedImageIndex = 2;
                    }
                    else if (child.IsObject)
                    {
                        childNode.ImageIndex = 0;
                        childNode.SelectedImageIndex = 0;
                        // Add dummy node for expandable objects
                        childNode.Nodes.Add(new TreeNode("Loading...") { Tag = null });
                    }

                    parentNode.Nodes.Add(childNode);
                }

                if (childNodes.Count == 0)
                {
                    parentNode.Nodes.Add(new TreeNode("(Empty)") { Tag = null });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to browse node: {parentNode.Text}", ex);
                parentNode.Nodes.Clear();
                parentNode.Nodes.Add(new TreeNode($"Error: {ex.Message}") { Tag = null });
            }
        }

        private void treeNodes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var nodeInfo = e.Node.Tag as NodeInfo;
            if (nodeInfo != null)
            {
                UpdateNodeDetails(nodeInfo);
                _selectedNode = nodeInfo;
                btnSelect.Enabled = nodeInfo.IsVariable;
            }
        }

        private async void UpdateNodeDetails(NodeInfo nodeInfo)
        {
            try
            {
                lstDetails.Items.Clear();
                
                lstDetails.Items.Add(new ListViewItem(new[] { "Node ID", nodeInfo.NodeId.ToString() }));
                lstDetails.Items.Add(new ListViewItem(new[] { "Display Name", nodeInfo.DisplayName }));
                lstDetails.Items.Add(new ListViewItem(new[] { "Browse Name", nodeInfo.BrowseName }));
                lstDetails.Items.Add(new ListViewItem(new[] { "Node Class", nodeInfo.NodeClass.ToString() }));

                if (nodeInfo.IsVariable)
                {
                    try
                    {
                        var value = await _opcClient.ReadNodeAsync(nodeInfo.NodeId);
                        nodeInfo.Value = value;
                        lstDetails.Items.Add(new ListViewItem(new[] { "Value", value?.ToString() ?? "null" }));
                        
                        // Read additional attributes
                        var nodesToRead = new ReadValueIdCollection
                        {
                            new ReadValueId { NodeId = nodeInfo.NodeId, AttributeId = Attributes.DataType },
                            new ReadValueId { NodeId = nodeInfo.NodeId, AttributeId = Attributes.ValueRank },
                            new ReadValueId { NodeId = nodeInfo.NodeId, AttributeId = Attributes.Description }
                        };

                        _opcClient.Session.Read(
                            null,
                            0,
                            TimestampsToReturn.None,
                            nodesToRead,
                            out DataValueCollection values,
                            out DiagnosticInfoCollection diagnosticInfos);

                        if (StatusCode.IsGood(values[0].StatusCode))
                        {
                            var dataTypeId = (NodeId)values[0].Value;
                            lstDetails.Items.Add(new ListViewItem(new[] { "Data Type", dataTypeId.ToString() }));
                        }

                        if (StatusCode.IsGood(values[1].StatusCode))
                        {
                            lstDetails.Items.Add(new ListViewItem(new[] { "Value Rank", values[1].Value.ToString() }));
                        }

                        if (StatusCode.IsGood(values[2].StatusCode) && values[2].Value != null)
                        {
                            var description = ((LocalizedText)values[2].Value).Text;
                            lstDetails.Items.Add(new ListViewItem(new[] { "Description", description }));
                        }
                    }
                    catch (Exception ex)
                    {
                        lstDetails.Items.Add(new ListViewItem(new[] { "Value", $"Error: {ex.Message}" }));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to update node details", ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            InitializeTreeView();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (_selectedNode != null && _selectedNode.IsVariable)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a variable node", "Invalid Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // Implement search functionality if needed
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter search text", "Search", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SearchNodes(treeNodes.Nodes, searchText);
        }

        private bool SearchNodes(TreeNodeCollection nodes, string searchText)
        {
            bool found = false;
            
            foreach (TreeNode node in nodes)
            {
                if (node.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    node.BackColor = System.Drawing.Color.Yellow;
                    node.EnsureVisible();
                    found = true;
                }
                else
                {
                    node.BackColor = System.Drawing.Color.White;
                }

                if (node.Nodes.Count > 0 && node.Nodes[0].Tag != null)
                {
                    if (SearchNodes(node.Nodes, searchText))
                    {
                        found = true;
                    }
                }
            }

            return found;
        }
    }
}