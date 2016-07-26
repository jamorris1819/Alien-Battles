﻿#if UNITY_WEBPLAYER

#else

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using UnityEditor;
using UnityEngine;


namespace Tiled2Unity
{
    // Partial class for the importer that deals with Materials
    partial class ImportTiled2Unity
    {
        // We need to call this while the renderers on the model is having its material assigned to it
        // This is invoked for every submesh in the .obj wavefront mesh
        public Material FixMaterialForMeshRenderer(string objName, Renderer renderer)
        {
            string xmlPath = GetXmlImportAssetPath(objName);
            ImportBehaviour importBehavior = ImportBehaviour.FindOrCreateImportBehaviour(xmlPath);

            // The mesh to match
            string meshName = renderer.name;

            // Increment our progress bar
            importBehavior.IncrementProgressBar(String.Format("Assign material: {0}", meshName));

            // Find an assignment that matches the mesh renderer
            var assignMaterials = importBehavior.XmlDocument.Root.Elements("AssignMaterial");
            XElement match = assignMaterials.FirstOrDefault(el => el.Attribute("mesh").Value == meshName);

            if (match == null)
            {
                // The names of our meshes in the AssignMaterials elements may be wrong
                // This happened before when Unity replaced whitespace with underscore in our named meshes
                // That case is handled now, but there may be others
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Could not find mesh named '{0}' for material matching\n", renderer.name);
                string choices = String.Join("\n  ", assignMaterials.Select(m => m.Attribute("mesh").Value).ToArray());
                builder.AppendFormat("Choices are:\n  {0}", choices);

                Debug.LogError(builder.ToString());
                return null;
            }

            string materialName = match.Attribute("material").Value + ".mat";
            string materialPath = GetMaterialAssetPath(materialName);

            // Assign the material
            Material material = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
            if (material == null)
            {
                Debug.LogError(String.Format("Could not find material: {0}", materialName));
            }

            // Do we have an alpha color key?
            string htmlColor = ImportUtils.GetAttributeAsString(match, "alphaColorKey", "");
            if (!String.IsNullOrEmpty(htmlColor))
            {
                // Take for granted color is in the form '#RRGGBB'
                byte r = byte.Parse(htmlColor.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(htmlColor.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(htmlColor.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                Color color = new Color32(r, g, b, 255);
                material.SetColor("_AlphaColorKey", color);
            }

            return material;
        }
    }
}
#endif