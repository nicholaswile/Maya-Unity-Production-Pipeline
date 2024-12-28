import maya.cmds as cmds
import os
import time

def export_and_notify():
    # Get the current scene name
    scene_name = cmds.file(q=True, sceneName=True)
    
    # Define the export path
    export_path = r"C:\\Path\\To\\Unity\\Project\\Assets\\Models"
    
    # Export the model as FBX
    file_name = os.path.splitext(os.path.basename(scene_name))[0] + ".fbx"
    export_file = os.path.join(export_path, file_name)
    cmds.file(export_file, force=True, options="v=0;", typ="FBX export", pr=True, es=True)
    
    # Create a notification file
    notification_file = os.path.join(export_path, "update_notification.txt")
    with open(notification_file, 'w') as f:
        f.write(str(time.time()))

# Add a script job to run on scene save
cmds.scriptJob(event=["SceneSaved", export_and_notify])