using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Engine;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.PhysicsSystem;
using Engine.Utils;
using Engine.Utils.Editor;
using EditorBase;
namespace Engine.Utils
{
    public partial class DebugWindow : Form
    {

        private List<object> objectList = new List<object>();
        private List<object> customObjectList = new List<object>();

        private static DebugWindow instance;

        private object currentObject;

        public static DebugWindow Instance
        {
            get 
            {
                if (instance == null)
                    instance = new DebugWindow();
                return instance; 
            }
        }

        public DebugWindow()
        {

            if (instance != null)
                Log.Fatal("Debug Window already created");

            instance = this;

            InitializeComponent();
        }

        private void DebugForm_Load(object sender, EventArgs e)
        {
            RefreshAll();

            ResourceUtils.OnUITypeEditorEditValue += new ResourceUtils.OnUITypeEditorEditValueDelegate(ResourceUtils_OnUITypeEditorEditValue);

            ResourceTypeManager.Init();

            //Register default types
            ResourceTypeManager.Instance.Register(new ResourceType("Texture", "Texture",
     new string[] { 
					"jpg", "jif", "jpeg", "jpe", "tga", "targa", "dds", "png", "bmp", "psd", "hdr", "ico", 
					"gif", "tif", "tiff", "exr", "j2k", "j2c", "jp2" },
     DefaultResourceTypeImages.GetByName("Texture_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("Mesh", "Mesh",
                new string[] { "mesh" }, DefaultResourceTypeImages.GetByName("Mesh_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("PhysicsModel", "Physics Model",
                new string[] { "physics" }, DefaultResourceTypeImages.GetByName("PhysicsModel_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("EntityType", "Entity Type",
                new string[] { "type" }, DefaultResourceTypeImages.GetByName("EntityType_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("ParticleSystem", "Particle System",
                new string[] { "particle" },
                DefaultResourceTypeImages.GetByName("ParticleSystem_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("Material", "Material",
                new string[] { "highMaterial" }, DefaultResourceTypeImages.GetByName("Material_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("GUI", "Graphic User Interface",
                new string[] { "gui" }, DefaultResourceTypeImages.GetByName("Gui_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("Sound", "Sound",
                new string[] { "ogg", "wav" }, DefaultResourceTypeImages.GetByName("Sound_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("Video", "Video",
                new string[] { "ogv" }, DefaultResourceTypeImages.GetByName("Video_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("Language", "Language",
                new string[] { "language" }, DefaultResourceTypeImages.GetByName("Language_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("Skeleton", "Skeleton",
                new string[] { "skeleton" }, DefaultResourceTypeImages.GetByName("Skeleton_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("FontDefinition", "Font Definition",
                new string[] { "fontDefinition" }, DefaultResourceTypeImages.GetByName("FontDefinition_16")));
            ResourceTypeManager.Instance.Register(new ResourceType("AnimationTree", "Animation Tree",
                new string[] { "animationTree" }, DefaultResourceTypeImages.GetByName("AnimationTree_16")));

        }

        void ResourceUtils_OnUITypeEditorEditValue(ResourceUtils.ResourceUITypeEditorEditValueEventHandler e)
        {
            ResourceType resourceType = ResourceTypeManager.Instance.GetByName(e.ResourceTypeName);
            if (resourceType == null)
                Log.Fatal("Resource type not defined \"{0}\"", e.ResourceTypeName);

            ChooseResourceForm dialog = new ChooseResourceForm(resourceType,
                true, e.ShouldAddDelegate, e.ResourceName,true);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                e.ResourceName = dialog.FilePath;
                e.Modified = true;
            }
        }

        void RefreshObjectList()
        {

            objectList.Clear();

            if (currentObject == null)
                currentObject = Map.Instance;

            if (currentObject is Map)
            {

                foreach (Entity entity in Map.Instance.Children)
                {
                    objectList.Add(entity);
                }

            }
            else if (currentObject is Entity)
            {

                Entity currentEntity = currentObject as Entity;

                if (currentEntity != null)
                {

                    foreach (Entity entity in currentEntity.Children)
                    {
                        objectList.Add(entity);
                    }

                }

                return;

            }

            foreach (object obj in customObjectList)
            {
                objectList.Add(obj);
            }

        }

        public void AddCustomObject(object customObject)
        {
            if (!customObjectList.Contains(customObject))
                customObjectList.Add(customObject);
        }

        public void RemoveCustomObject(object customObject)
        {
            customObjectList.Remove(customObject);
        }

        public void ClearCustomObjectList()
        {
            customObjectList.Clear();
        }

        void RefreshEntityList()
        {

            entityList.SuspendLayout();

            entityList.Items.Clear();

                foreach (object obj in objectList)
                {

                    Entity entity = obj as Entity;

                    if (entity != null)
                    {

                        string name = entity.Name != string.Empty ? entity.Name : entity.Type.Name;

                        string type = entity.Type.Name;

                        if (textBoxFilter.Text != string.Empty &&
                            !(name.ToLower().Contains(textBoxFilter.Text) ||
                            type.ToLower().Contains(textBoxFilter.Text)))
                            continue;

                        ListViewItem item = new ListViewItem(name);

                        item.SubItems.Add(type);

                        item.SubItems.Add(entity.Children.Count.ToString());

                        item.Tag = entity;

                        entityList.Items.Add(item);

                    }
                    else
                    {

                        string name = obj.ToString();

                        string type = obj.GetType().Name;

                        if (textBoxFilter.Text != string.Empty &&
                            !(name.ToLower().Contains(textBoxFilter.Text) ||
                            type.ToLower().Contains(textBoxFilter.Text)))
                            continue;

                        ListViewItem item = new ListViewItem(name);

                        item.SubItems.Add(type);

                        entityList.Items.Add(item);

                    }


                    

                }

            entityList.ResumeLayout();

        }

        public void RefreshAll()
        {
            RefreshObjectList();
            RefreshEntityList();
        }

        private void DebugWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }


        private void DebugWindow_MouseLeave(object sender, EventArgs e)
        {
            //DeadWake.GameEngineApp.Instance.MouseRelativeMode = mouseRealativeMode;
            //DeadWake.GameEngineApp.Instance.ShowSystemCursor = showSystemCursor;
            //EntitySystemWorld.Instance.Simulation = true;
        }

        private void DebugWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Visible = false;
        }


        private void entityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            propEditor.SelectedObject = propEditor.SelectedObject = null;

            typeEditor.SelectedObject = typeEditor.SelectedObjects = null;


            if (entityList.SelectedItems.Count == 1)
            {

                propEditor.SelectedObject = entityList.SelectedItems[0].Tag;

                Entity entity = entityList.SelectedItems[0].Tag as Entity;

                if (entity != null)
                {
                    typeEditor.SelectedObject = entity.Type;
                    propEditor.SelectedObject = entity;
                }
            }

            else if (entityList.SelectedItems.Count > 1)
            {

                //object[] entities = new object[entityList.SelectedItems.Count - 1];

                List<object> entities = new List<object>();
                List<object> types = new List<object>();

                foreach (ListViewItem item in entityList.SelectedItems)
                {
                    entities.Add(item.Tag);

                    Entity entity = item.Tag as Entity;

                    if (entity != null)
                        types.Add(entity.Type);


                }

                propEditor.SelectedObjects = entities.ToArray();
                typeEditor.SelectedObjects = types.ToArray();
            }

        }

        private void DebugWindow_Activated(object sender, EventArgs e)
        {
            /*mouseRealativeMode = EngineApp.Instance.MouseRelativeMode;
            showSystemCursor = EngineApp.Instance.ShowSystemCursor;
            simulation = EntitySystemWorld.Instance.Simulation;
            DeadWake.GameEngineApp.Instance.MouseRelativeMode = false;
            DeadWake.GameEngineApp.Instance.ShowSystemCursor = true;
            EntitySystemWorld.Instance.Simulation = false;*/
        }


        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            RefreshEntityList();
        }

        private void entityList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Visible = false;

            if (e.KeyCode == Keys.Back)
            {
                GoOneUp();
            }

        }

        private void textBoxFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Visible = false;
        }

        private void checkBoxTopMost_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = checkBoxTopMost.Checked;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void GoOneUp()
        {
            if (currentObject == null)
            {
                currentObject = Map.Instance;
                RefreshAll();
                return;
            }

            Entity entity = currentObject as Entity;

            if (entity == null)
                return;

            if (entity.Parent == null)
            {
                currentObject = Map.Instance;
                RefreshAll();
                return;
            }

            currentObject = entity.Parent;

            RefreshAll();

        }

        private void entityList_DoubleClick(object sender, EventArgs e)
        {

            if (entityList.SelectedItems.Count == 0 || entityList.SelectedItems.Count > 1)
                return;

            ListViewItem item = entityList.SelectedItems[0];


            Entity entity = item.Tag as Entity;

            if (entity != null && entity.Children.Count > 0)
            {
                currentObject = entity;
                RefreshAll();
            }

        }

        private void buttonUP_Click(object sender, EventArgs e)
        {
            GoOneUp();
        }

        private void buttonSaveType_Click(object sender, EventArgs e)
        {
            SaveEntityType();
        }

        public object[] GetCurrentSelection()
        {
            List<object> objectSelection = new List<object>();

            foreach (ListViewItem item in entityList.SelectedItems)
            {
                objectSelection.Add(item.Tag);
            }

            return objectSelection.ToArray();

        }

        public void SaveEntityType()
        {
            object[] entities = GetCurrentSelection();

            foreach (Entity entity in entities)
            {
                EntityTypes.Instance.SaveTypeToFile(entity.Type);
            }

        }

        public void SaveEntitiesType(List<Entity> entities)
        {

            foreach (Entity entity in entities)
            {
                EntityTypes.Instance.SaveTypeToFile(entity.Type);
            }

        }

        public void SaveEntityType(Entity entity)
        {
            if (entity != null)
            {
                EntityTypes.Instance.SaveTypeToFile(entity.Type);
            }
        }

        public void SaveEntityType(EntityType type)
        {
            if (type != null)
            {
                EntityTypes.Instance.SaveTypeToFile(type);
            }
        }

        private void checkBoxTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTransparent.Checked)
                Opacity = 0.75;
            else
                Opacity = 1.0;
        }



        private void propEditor_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {

        }

        private void propEditor_Click(object sender, EventArgs e)
        {
            
        }

    }
}
