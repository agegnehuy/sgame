using System.Collections.Generic;
using System.IO;
using SGame.Gameplay;
using SGame.Gameplay.Walk;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

namespace SGame.Editor
{
    /// <summary>
    /// One-click scene builder for the "Walk from Home to School" mission MVP.
    /// Creates Assets/_Game/Scenes/P1_M1_Walk.unity from scratch with primitive
    /// geometry, materials, and all gameplay components wired together.
    /// </summary>
    public static class WalkSceneBuilder
    {
        private const string ScenePath = "Assets/_Game/Scenes/P1_M1_Walk.unity";

        // Layout (world units = meters)
        private const float RoadWidth = 8f;            // crosswalk depth (Z)
        private const float RoadLength = 70f;          // along X
        private const float SidewalkWidth = 3f;
        private const float PathWidth = 3f;
        // Home is far back from the road so the player has a long runway to
        // observe at least one full RED→GREEN→RED light cycle while running
        // toward the zebra. At ~5.5 m/s the kid takes ~8.5s to reach the curb,
        // which lines up with one full traffic-light cycle.
        private const float HomeZ = -55f;
        private const float SchoolZ = 26f;
        // Compact loop — cars wrap around just outside the visible window so
        // the road never appears empty. With 64m the cars used to vanish for
        // ~6s of every cycle, which looked like "no cars are moving" even
        // though they were just off-screen waiting to loop.
        private const float CarLoopStartX = -22f;
        private const float CarLoopEndX   =  22f;
        private const float SouthLaneZ = -1.8f;
        private const float NorthLaneZ =  1.8f;

        // Material 3-ish palette (loose match to the Stitch design tokens)
        private static readonly Color GroundColor     = new Color(0.55f, 0.80f, 0.45f);
        private static readonly Color RoadColor       = new Color(0.20f, 0.20f, 0.22f);
        private static readonly Color SidewalkColor   = new Color(0.83f, 0.83f, 0.83f);
        private static readonly Color CrosswalkColor  = new Color(0.97f, 0.97f, 0.97f);
        private static readonly Color HomeColor       = new Color(0.95f, 0.65f, 0.40f);
        private static readonly Color HomeRoofColor   = new Color(0.62f, 0.20f, 0.18f);
        private static readonly Color SchoolColor     = new Color(0.28f, 0.55f, 0.95f);
        private static readonly Color SchoolRoofColor = new Color(0.18f, 0.30f, 0.55f);
        private static readonly Color PoleColor       = new Color(0.20f, 0.20f, 0.22f);
        private static readonly Color PlayerColor     = new Color(1.00f, 0.84f, 0.20f);
        private static readonly Color PlayerHeadColor = new Color(0.95f, 0.78f, 0.62f);

        // UI palette — Stitch "Safe Steps Addis" tokens
        private static readonly Color UiCream         = new Color(0.953f, 0.973f, 0.988f, 1f);    // #F3F8FC
        private static readonly Color UiNavy          = new Color(0.106f, 0.176f, 0.243f, 1f);    // #1B2D3E
        private static readonly Color UiNavySoft      = new Color(0.302f, 0.400f, 0.467f, 1f);    // muted body
        private static readonly Color UiYellow        = new Color(1.000f, 0.847f, 0.208f, 1f);    // #FFD835
        private static readonly Color UiYellowShadow  = new Color(0.792f, 0.631f, 0.000f, 1f);    // #C9A100
        private static readonly Color UiGreen         = new Color(0.310f, 0.796f, 0.376f, 1f);    // #4FCB60
        private static readonly Color UiGreenShadow   = new Color(0.180f, 0.659f, 0.306f, 1f);    // #2EA84E
        private static readonly Color UiGreenDeep     = new Color(0.106f, 0.486f, 0.188f, 1f);    // dark text accent
        private static readonly Color UiRed           = new Color(0.753f, 0.271f, 0.271f, 1f);    // #C04545
        private static readonly Color UiRedShadow     = new Color(0.557f, 0.180f, 0.180f, 1f);    // #8E2E2E
        private static readonly Color UiBluePale      = new Color(0.863f, 0.914f, 0.969f, 1f);    // #DCE9F7
        private static readonly Color UiBlueAccent    = new Color(0.169f, 0.357f, 0.851f, 1f);    // #2B5BD9
        private static readonly Color UiCardWhite     = new Color(1.000f, 1.000f, 1.000f, 1f);
        private static readonly Color UiOverlayDim    = new Color(0.0f, 0.0f, 0.0f, 0.45f);
        private static readonly Color UiTLBackdrop    = new Color(0.180f, 0.224f, 0.275f, 1f);    // dark housing
        private static readonly Color CarColor1       = new Color(0.95f, 0.30f, 0.30f);
        private static readonly Color CarColor2       = new Color(0.30f, 0.60f, 0.95f);
        private static readonly Color CarColor3       = new Color(0.95f, 0.75f, 0.20f);
        private static readonly Color CarColor4       = new Color(0.50f, 0.35f, 0.85f);
        private static readonly Color CarColor5       = new Color(0.90f, 0.50f, 0.20f);
        private static readonly Color CarColor6       = new Color(0.20f, 0.70f, 0.70f);

        [MenuItem("SGame/Build Walk Scene (Home → School)")]
        public static void Build()
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("WalkSceneBuilder",
                    "Stop Play mode first, then run this.", "OK");
                return;
            }

            var dir = Path.GetDirectoryName(ScenePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            BuildQualitySettings();
            BuildLighting();
            BuildReflectionProbe();
            BuildPostProcessing();
            EnsureProceduralTextures();
            BuildEnvironment(out Transform crosswalkCenter);
            BuildSideBuildings();
            BuildLampposts();
            BuildStreetSigns();
            BuildStreetProps();
            BuildTrafficLight(out TrafficLight3D trafficLight, out MeshRenderer lightHead);
            var cars = BuildVehicles(trafficLight);
            BuildPlayer(out GameObject playerGo, out KidPlayer kidPlayer);
            BuildNPCs();
            BuildCrosswalkNPCs(trafficLight);
            BuildFollowCamera(playerGo.transform);
            BuildMissionManager(out MissionController missionController);
            var crosswalkZone = BuildCrosswalkTriggers(missionController, trafficLight, kidPlayer);
            var schoolGoal = BuildSchoolGoal(crosswalkZone);
            var homeGoal = BuildHomeGoal(crosswalkZone);
            var classroomAnchor = BuildSchoolInterior();
            var roundTrip = BuildRoundTripController(schoolGoal, homeGoal, crosswalkZone, kidPlayer, classroomAnchor);

            // (Removed: the floating 3D chevron above the kid's head felt
            // intrusive. The objective banner at the top of the HUD still
            // tells the player what to do at each milestone.)

            // Parked cars + extra "lived-in" props (bus stop, planters,
            // hydrant, trash bags) on top of the existing street furniture
            // (mailbox, trash cans, benches) that BuildStreetProps() above
            // already placed.
            BuildParkedCars();
            BuildExtraStreetProps();

            // Procedural ambient audio (city rumble, occasional horns,
            // distant chatter, footsteps tied to the kid's running cadence).
            BuildAmbientAudio(kidPlayer);

            BuildCarReporter(missionController, crosswalkCenter, cars);
            trafficLight.Configure(missionController, lightHead);
            BuildHUD(missionController, trafficLight, crosswalkZone, schoolGoal, roundTrip);

            var boot = new GameObject("WalkMissionBootstrapper");
            var bootScript = boot.AddComponent<WalkMissionBootstrapper>();
            bootScript.Bind(missionController);

            // Silent runtime watchdog — prevents soft-locks where Time.timeScale
            // gets stuck at slow-mo, or the kid's walkingAllowed flag stays false
            // across a Play session. No on-screen UI; pure safety net.
            var watchdog = new GameObject("LiveDebugHUD");
            watchdog.AddComponent<LiveDebugHUD>();

            // Mark non-moving renderers as static so Unity can batch them.
            // This is the single biggest mobile perf win — collapses dozens of
            // separate draw calls (sidewalks, buildings, fences, signs, trees)
            // into a small set of combined meshes.
            MarkSceneStaticForBatching();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);

            EditorUtility.DisplayDialog("WalkSceneBuilder",
                "Built " + ScenePath + ".\n\n" +
                "Press ▶ Play.\n" +
                "• Kid auto-RUNS from frame 1 (accel/decel + walk↔run blend).\n" +
                "• STOP halts her, GO resumes.\n" +
                "• Cars-light GREEN = cars go, peds must STOP.\n" +
                "• Cars-light RED   = cars stop, peds may GO.\n" +
                "• On the zebra ANY time the light is GREEN → ACCIDENT.\n\n" +
                "World life:\n" +
                "  • 6 walking NPCs + 2 crosswalk-using NPCs (yield to each\n" +
                "    other and to the player).\n" +
                "  • 4 parked cars on the shoulders, bus stop with bench &\n" +
                "    sign, planters, fire hydrant, trash bags.\n" +
                "  • Procedural AMBIENT AUDIO: city rumble loop, occasional\n" +
                "    distant horns, distant chatter, kid's footsteps tied to\n" +
                "    her running cadence.\n\n" +
                "Round trip:\n" +
                "  1) Run from home to the school front door.\n" +
                "  2) Fade to black → fade in to a CLASSROOM diorama: teacher\n" +
                "     at the blackboard, 7 classmates, the kid at her own\n" +
                "     labelled desk. 3 road-safety lessons display in turn.\n" +
                "  3) Fade out → kid emerges from the back door, walks the\n" +
                "     courtyard path, joins the north sidewalk, walks WEST\n" +
                "     along the sidewalk to the zebra, crosses, walks the\n" +
                "     south sidewalk + home path to HOME.\n" +
                "  4) She NEVER walks on grass — yellow chevron breadcrumbs\n" +
                "     mark the entire return route.\n\n" +
                "Mobile: shadows + AO + motion blur + chromatic aberration are\n" +
                "stripped on Android/iOS; all static geometry is BatchingStatic.",
                "Open it");
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        // ── Lighting ─────────────────────────────────────────────────────────

        // ── Quality settings — pick a tier appropriate to the build target ──
        // Editor / PC / WebGL get the high-quality preset (shadow cascades,
        // 4× MSAA, real-time reflections). Android gets a lean mobile preset
        // (1 cascade, 30m shadow distance, 2× MSAA, 2 pixel lights) so the
        // game runs at 60 fps on mid-range phones without losing the road-
        // safety story the visuals are telling.
        private static void BuildQualitySettings()
        {
            int top = QualitySettings.names.Length - 1;
            if (top >= 0)
            {
                QualitySettings.SetQualityLevel(top, applyExpensiveChanges: true);
            }

            bool isMobile = IsMobileBuildTarget();

            QualitySettings.realtimeReflectionProbes = !isMobile;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowResolution = isMobile
                ? ShadowResolution.Medium
                : ShadowResolution.High;
            QualitySettings.shadowProjection = ShadowProjection.StableFit;
            QualitySettings.shadowDistance = isMobile ? 35f : 110f;
            QualitySettings.shadowCascades = isMobile ? 1 : 4;
            QualitySettings.softParticles = !isMobile;
            QualitySettings.pixelLightCount = isMobile ? 2 : Mathf.Max(4, QualitySettings.pixelLightCount);
            QualitySettings.antiAliasing = isMobile ? 2 : 4;

            // Cap the frame rate on mobile so the GPU isn't burning battery
            // pushing 90+ fps when the game is designed around 60.
            if (isMobile)
            {
                Application.targetFrameRate = 60;
                QualitySettings.vSyncCount = 0;
            }

            // Mark project settings dirty so the changes persist to disk.
            AssetDatabase.SaveAssets();
        }

        private static bool IsMobileBuildTarget()
        {
            var t = EditorUserBuildSettings.activeBuildTarget;
            return t == BuildTarget.Android || t == BuildTarget.iOS;
        }

        // ── Static batching ──────────────────────────────────────────────────
        //
        // Walks the scene root and flips StaticEditorFlags on everything that
        // shouldn't move (environment, buildings, fences, props, signs). The
        // player, cars, NPCs, coins, traffic light bulbs, the objective
        // arrow, and the HUD are skipped.
        private static readonly string[] DynamicRoots = new[]
        {
            "Player", "Cars", "NPCs", "CrosswalkNPCs", "Coins", "ObjectiveArrow",
            "HUDCanvas", "FadeOverlay", "RoundTripController", "SchoolGoal",
            "HomeGoal", "TrafficLight", "EventSystem", "Main Camera",
            "WalkMissionBootstrapper", "ObjectiveTargets", "MissionController",
            "CarDistanceReporter", "CrosswalkCenter"
        };

        private static void MarkSceneStaticForBatching()
        {
            var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                if (System.Array.IndexOf(DynamicRoots, root.name) >= 0) continue;
                MarkStaticRecursive(root.transform);
            }
        }

        private static void MarkStaticRecursive(Transform t)
        {
            // Skip dynamic sub-trees nested under environment.
            if (System.Array.IndexOf(DynamicRoots, t.name) >= 0) return;

            // Anything with a script that ticks at runtime should NOT be static.
            // (Coins spin, traffic lights cycle, etc.)
            var behaviours = t.GetComponents<MonoBehaviour>();
            bool hasGameplayScript = false;
            for (int i = 0; i < behaviours.Length; i++)
            {
                var mb = behaviours[i];
                if (mb == null) continue;
                if (mb is Coin || mb is CarMover || mb is KidPlayer ||
                    mb is HumanoidLimbAnimator || mb is TrafficLight3D ||
                    mb is NPCWalker || mb is CrosswalkNPCWalker ||
                    mb is ObjectiveArrow)
                {
                    hasGameplayScript = true;
                    break;
                }
            }
            if (!hasGameplayScript)
            {
                GameObjectUtility.SetStaticEditorFlags(t.gameObject,
                    StaticEditorFlags.BatchingStatic |
                    StaticEditorFlags.OccluderStatic |
                    StaticEditorFlags.OccludeeStatic |
                    StaticEditorFlags.ReflectionProbeStatic);
            }

            for (int i = 0; i < t.childCount; i++)
            {
                MarkStaticRecursive(t.GetChild(i));
            }
        }

        private static void BuildLighting()
        {
            // Warm late-afternoon sun. Slightly lower angle gives long, soft
            // shadows that anchor objects to the ground — a huge realism cue.
            var sun = new GameObject("Sun");
            var light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.45f;
            light.color = new Color(1.00f, 0.92f, 0.78f);
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.90f;
            light.shadowBias = 0.02f;
            light.shadowNormalBias = 0.6f;
            sun.transform.rotation = Quaternion.Euler(38f, -42f, 0f);
            QualitySettings.shadowDistance = 110f;
            QualitySettings.shadowCascades = 4;

            // Procedural sky with explicit sun position so the haze + sky color
            // come from the same direction the directional light comes from.
            var skyShader = Shader.Find("Skybox/Procedural");
            if (skyShader != null)
            {
                var skyMat = new Material(skyShader);
                skyMat.SetFloat("_SunSize", 0.045f);
                skyMat.SetFloat("_SunSizeConvergence", 5f);
                skyMat.SetFloat("_AtmosphereThickness", 1.05f);
                skyMat.SetColor("_SkyTint", new Color(0.55f, 0.70f, 0.90f));
                skyMat.SetColor("_GroundColor", new Color(0.36f, 0.40f, 0.34f));
                skyMat.SetFloat("_Exposure", 1.30f);
                RenderSettings.skybox = skyMat;
                RenderSettings.sun = light;
            }
            else
            {
                var fallback = Resources.GetBuiltinResource<Material>("Default-Skybox.mat");
                if (fallback != null) RenderSettings.skybox = fallback;
            }

            // Ambient comes from the procedural skybox so light bounces feel
            // physically plausible instead of flat trilight.
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 1.10f;
            RenderSettings.reflectionIntensity = 1.0f;
            DynamicGI.UpdateEnvironment();

            // Distance fog tinted to match the sky horizon for hazy depth.
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.74f, 0.82f, 0.92f);
            RenderSettings.fogStartDistance = 30f;
            RenderSettings.fogEndDistance = 140f;
        }

        // ── Reflection probe ─────────────────────────────────────────────────
        // Captures the sky + nearby geometry so car paint, windows, and the
        // traffic light housing show a real reflection instead of looking matte.

        private static void BuildReflectionProbe()
        {
            var probeGo = new GameObject("ReflectionProbe");
            probeGo.transform.position = new Vector3(0f, 4f, 0f);
            var probe = probeGo.AddComponent<ReflectionProbe>();
            probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
            probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
            probe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.IndividualFaces;
            probe.size = new Vector3(80f, 25f, 40f);
            probe.center = Vector3.zero;
            probe.intensity = 1f;
            probe.boxProjection = true;
            probe.resolution = 256;
            probe.clearFlags = UnityEngine.Rendering.ReflectionProbeClearFlags.Skybox;
            probe.nearClipPlane = 0.3f;
            probe.farClipPlane = 250f;
        }

        // ── Post-processing volume + profile ────────────────────────────────
        // Adds Bloom, ACES tonemapping + color grading, ambient occlusion,
        // vignette, and motion blur. The camera receives the PostProcessLayer
        // in BuildFollowCamera so these effects render.

        private static void BuildPostProcessing()
        {
#if UNITY_POST_PROCESSING_STACK_V2
            bool isMobile = IsMobileBuildTarget();

            // Profile asset must exist on disk BEFORE we add the effect
            // ScriptableObjects to it, otherwise the SO references serialize
            // as fileID:0 and the effects vanish on save.
            const string profilePath = "Assets/_Game/Scenes/WalkPostProcess.asset";
            var dir = Path.GetDirectoryName(profilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (AssetDatabase.LoadAssetAtPath<PostProcessProfile>(profilePath) != null)
            {
                AssetDatabase.DeleteAsset(profilePath);
            }
            var profile = ScriptableObject.CreateInstance<PostProcessProfile>();
            profile.name = "WalkPostProcess";
            AssetDatabase.CreateAsset(profile, profilePath);

            // Bloom is the biggest "real-world feel" win and cheap-ish even on
            // mobile (we drop intensity + raise threshold so only emissive
            // signs and headlights bloom).
            var bloom = AddEffect<Bloom>(profile);
            bloom.enabled.Override(true);
            bloom.intensity.Override(isMobile ? 1.6f : 3.2f);
            bloom.threshold.Override(isMobile ? 0.95f : 0.70f);
            bloom.softKnee.Override(0.65f);
            bloom.diffusion.Override(isMobile ? 5f : 8f);
            bloom.color.Override(new Color(1.0f, 0.95f, 0.85f));

            var grading = AddEffect<ColorGrading>(profile);
            grading.enabled.Override(true);
            grading.gradingMode.Override(isMobile
                ? GradingMode.LowDefinitionRange  // LDR is cheaper on mobile GPUs
                : GradingMode.HighDefinitionRange);
            grading.tonemapper.Override(Tonemapper.ACES);
            grading.postExposure.Override(0.55f);
            grading.contrast.Override(22f);
            grading.saturation.Override(28f);
            grading.temperature.Override(12f);
            grading.tint.Override(-4f);
            grading.colorFilter.Override(new Color(1.04f, 1.00f, 0.95f));

            // AO is *the* most expensive post-fx on mobile — skip it there.
            if (!isMobile)
            {
                var ao = AddEffect<AmbientOcclusion>(profile);
                ao.enabled.Override(true);
                ao.mode.Override(AmbientOcclusionMode.ScalableAmbientObscurance);
                ao.intensity.Override(1.40f);
                ao.radius.Override(0.8f);
                ao.color.Override(new Color(0.05f, 0.06f, 0.10f));
            }

            var vignette = AddEffect<Vignette>(profile);
            vignette.enabled.Override(true);
            vignette.mode.Override(VignetteMode.Classic);
            vignette.intensity.Override(isMobile ? 0.28f : 0.38f);
            vignette.smoothness.Override(0.45f);
            vignette.color.Override(new Color(0.03f, 0.04f, 0.08f));

            // Motion blur + chromatic aberration are bandwidth-heavy AND, more
            // critically, motion blur was smearing the moving cars into pale
            // ghost-blobs that looked stationary from the home end of the
            // scene. We intentionally keep these effects disabled.
            // (Re-enable later when chasing photo-realism on PC only.)

            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var volumeGo = new GameObject("GlobalPostProcess");
            var volume = volumeGo.AddComponent<PostProcessVolume>();
            volume.isGlobal = true;
            volume.priority = 1f;
            volume.sharedProfile = profile;
#else
            // Post-processing package not yet imported. Unity will auto-install
            // com.unity.postprocessing on the next package resolve; after that
            // the scene needs to be rebuilt to pick up the effects.
            Debug.Log("[WalkSceneBuilder] com.unity.postprocessing not loaded yet — " +
                      "let Unity finish importing it, then rebuild the scene.");
#endif
        }

#if UNITY_POST_PROCESSING_STACK_V2
        // Effect settings have to be SUB-ASSETS of the profile, otherwise they
        // serialize as fileID:0 and silently disappear at runtime.
        private static T AddEffect<T>(PostProcessProfile profile)
            where T : PostProcessEffectSettings
        {
            var settings = ScriptableObject.CreateInstance<T>();
            settings.name = typeof(T).Name;
            settings.hideFlags = HideFlags.HideInHierarchy;
            settings.enabled.value = true;
            profile.settings.Add(settings);
            AssetDatabase.AddObjectToAsset(settings, profile);
            return settings;
        }
#endif

        // ── Procedural textures ─────────────────────────────────────────────
        // Generate seamless asphalt / concrete / grass / brick textures and
        // save them to disk so materials can reference them stably. Skip
        // generation if the files already exist (regen is expensive).

        private const string GeneratedDir = "Assets/_Game/Generated";
        private const string AsphaltTexPath = GeneratedDir + "/Tex_Asphalt.png";
        private const string ConcreteTexPath = GeneratedDir + "/Tex_Concrete.png";
        private const string GrassTexPath = GeneratedDir + "/Tex_Grass.png";
        private const string BrickTexPath = GeneratedDir + "/Tex_Brick.png";
        private const string PlasterTexPath = GeneratedDir + "/Tex_Plaster.png";

        private static Texture2D _asphaltTex;
        private static Texture2D _concreteTex;
        private static Texture2D _grassTex;
        private static Texture2D _brickTex;
        private static Texture2D _plasterTex;

        private static void EnsureProceduralTextures()
        {
            if (!Directory.Exists(GeneratedDir)) Directory.CreateDirectory(GeneratedDir);

            EnsureTexture(AsphaltTexPath, 256, () => GenerateAsphalt(256));
            EnsureTexture(ConcreteTexPath, 256, () => GenerateConcrete(256));
            EnsureTexture(GrassTexPath, 256, () => GenerateGrass(256));
            EnsureTexture(BrickTexPath, 256, () => GenerateBrick(256));
            EnsureTexture(PlasterTexPath, 256, () => GeneratePlaster(256));

            _asphaltTex  = AssetDatabase.LoadAssetAtPath<Texture2D>(AsphaltTexPath);
            _concreteTex = AssetDatabase.LoadAssetAtPath<Texture2D>(ConcreteTexPath);
            _grassTex    = AssetDatabase.LoadAssetAtPath<Texture2D>(GrassTexPath);
            _brickTex    = AssetDatabase.LoadAssetAtPath<Texture2D>(BrickTexPath);
            _plasterTex  = AssetDatabase.LoadAssetAtPath<Texture2D>(PlasterTexPath);
        }

        private static void EnsureTexture(string path, int size, System.Func<Color32[]> gen)
        {
            if (File.Exists(path)) return;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, mipChain: true, linear: false);
            tex.SetPixels32(gen());
            tex.Apply(updateMipmaps: true);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.filterMode = FilterMode.Trilinear;
                importer.anisoLevel = 8;
                importer.maxTextureSize = 512;
                importer.SaveAndReimport();
            }
        }

        // Asphalt: dark gray base + Perlin grain + occasional darker stains.
        private static Color32[] GenerateAsphalt(int n)
        {
            var pixels = new Color32[n * n];
            float ox = Random.Range(0f, 100f), oy = Random.Range(0f, 100f);
            for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float u = x / (float)n;
                float v = y / (float)n;
                float grain = Mathf.PerlinNoise((ox + u) * 32f, (oy + v) * 32f);
                float blotch = Mathf.PerlinNoise((ox + u) * 4f, (oy + v) * 4f);
                float gray = 0.16f + grain * 0.10f - blotch * 0.05f;
                gray = Mathf.Clamp01(gray);
                byte g = (byte)(gray * 255f);
                pixels[y * n + x] = new Color32(g, g, (byte)(g + 3), 255);
            }
            return pixels;
        }

        // Concrete: light gray with subtle blotch and seams every 64px.
        private static Color32[] GenerateConcrete(int n)
        {
            var pixels = new Color32[n * n];
            float ox = Random.Range(0f, 100f), oy = Random.Range(0f, 100f);
            for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float u = x / (float)n;
                float v = y / (float)n;
                float grain = Mathf.PerlinNoise((ox + u) * 18f, (oy + v) * 18f);
                float blotch = Mathf.PerlinNoise((ox + u) * 3f, (oy + v) * 3f);
                float gray = 0.62f + grain * 0.10f - blotch * 0.06f;
                // Panel seams.
                bool seamX = (x % 64) == 0 || (x % 64) == 63;
                bool seamY = (y % 64) == 0 || (y % 64) == 63;
                if (seamX || seamY) gray -= 0.18f;
                gray = Mathf.Clamp01(gray);
                byte g = (byte)(gray * 255f);
                pixels[y * n + x] = new Color32(g, g, (byte)(g - 4), 255);
            }
            return pixels;
        }

        // Grass: green base + two-frequency Perlin variation + occasional yellow tips.
        private static Color32[] GenerateGrass(int n)
        {
            var pixels = new Color32[n * n];
            float ox = Random.Range(0f, 100f), oy = Random.Range(0f, 100f);
            for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float u = x / (float)n;
                float v = y / (float)n;
                float a = Mathf.PerlinNoise((ox + u) * 6f,  (oy + v) * 6f);
                float b = Mathf.PerlinNoise((ox + u) * 28f, (oy + v) * 28f);
                float val = 0.5f * a + 0.5f * b;
                Color baseG = new Color(0.32f + val * 0.12f, 0.55f + val * 0.18f, 0.22f + val * 0.10f);
                // Sparse yellow tips
                if (Random.value > 0.985f) baseG = new Color(0.78f, 0.72f, 0.32f);
                pixels[y * n + x] = new Color32(
                    (byte)(Mathf.Clamp01(baseG.r) * 255f),
                    (byte)(Mathf.Clamp01(baseG.g) * 255f),
                    (byte)(Mathf.Clamp01(baseG.b) * 255f), 255);
            }
            return pixels;
        }

        // Brick: alternating rows of bricks with mortar gaps.
        private static Color32[] GenerateBrick(int n)
        {
            var pixels = new Color32[n * n];
            int rowH = 32;
            int brickW = 64;
            int mortar = 4;
            Color mortarCol = new Color(0.74f, 0.72f, 0.66f);
            for (int y = 0; y < n; y++)
            {
                int rowIdx = y / rowH;
                int offset = (rowIdx % 2 == 0) ? 0 : brickW / 2;
                for (int x = 0; x < n; x++)
                {
                    int yInRow = y % rowH;
                    int xShift = (x + offset) % n;
                    int xInBrick = xShift % brickW;
                    bool isMortar = yInRow < mortar || xInBrick < mortar;
                    Color c;
                    if (isMortar)
                    {
                        c = mortarCol;
                    }
                    else
                    {
                        // Per-brick color variation seeded by brick coords.
                        int bx = xShift / brickW;
                        int by = rowIdx;
                        float h = Mathf.Repeat((bx * 53 + by * 91) * 0.013f, 1f);
                        float r = 0.55f + h * 0.20f;
                        float g = 0.25f + h * 0.10f;
                        float b = 0.18f + h * 0.07f;
                        // Mortar shadow gradient at the brick edges.
                        float edgeY = Mathf.SmoothStep(0f, 1f, (yInRow - mortar) / 4f) *
                                      Mathf.SmoothStep(0f, 1f, (rowH - 1 - yInRow) / 4f);
                        float edgeX = Mathf.SmoothStep(0f, 1f, (xInBrick - mortar) / 4f) *
                                      Mathf.SmoothStep(0f, 1f, (brickW - 1 - xInBrick) / 4f);
                        float edge = Mathf.Min(edgeY, edgeX);
                        c = new Color(r, g, b) * (0.78f + 0.22f * edge);
                    }
                    pixels[y * n + x] = new Color32(
                        (byte)(Mathf.Clamp01(c.r) * 255f),
                        (byte)(Mathf.Clamp01(c.g) * 255f),
                        (byte)(Mathf.Clamp01(c.b) * 255f), 255);
                }
            }
            return pixels;
        }

        // Plaster: warm off-white with low-frequency variation, for stucco walls.
        private static Color32[] GeneratePlaster(int n)
        {
            var pixels = new Color32[n * n];
            float ox = Random.Range(0f, 100f), oy = Random.Range(0f, 100f);
            for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float u = x / (float)n;
                float v = y / (float)n;
                float a = Mathf.PerlinNoise((ox + u) * 10f, (oy + v) * 10f);
                float b = Mathf.PerlinNoise((ox + u) * 60f, (oy + v) * 60f);
                float val = 0.88f + a * 0.06f - b * 0.04f;
                val = Mathf.Clamp01(val);
                byte r = (byte)(val * 235f);
                byte g = (byte)(val * 230f);
                byte bcol = (byte)(val * 220f);
                pixels[y * n + x] = new Color32(r, g, bcol, 255);
            }
            return pixels;
        }

        // Apply a textured PBR material. `tilingPerMeter` repeats the texture
        // once per world unit on each axis — set 0 to use UV directly.
        private static void ApplyTextured(MeshRenderer mr, Texture2D tex, Color tint,
            float metallic, float smoothness, Vector2 tiling)
        {
            if (mr == null) return;
            var shader = Shader.Find("Standard") ?? Shader.Find("Diffuse");
            var mat = new Material(shader);
            if (mat.HasProperty("_Color"))     mat.SetColor("_Color", tint);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", tint);
            if (mat.HasProperty("_MainTex"))   mat.SetTexture("_MainTex", tex);
            if (mat.HasProperty("_BaseMap"))   mat.SetTexture("_BaseMap", tex);
            if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
            mat.mainTextureScale = tiling;
            mr.sharedMaterial = mat;
        }

        // ── Environment (ground, road, sidewalks, crosswalk, home, school) ───

        private static void BuildEnvironment(out Transform crosswalkCenter)
        {
            var env = new GameObject("Environment").transform;

            // Ground — textured grass, tiled densely so individual blades read.
            var ground = CreatePlane("Ground", env, new Vector3(0f, 0f, 0f),
                new Vector3(10f, 1f, 10f), GroundColor);
            ApplyTextured(ground.GetComponent<MeshRenderer>(), _grassTex,
                new Color(0.92f, 1.00f, 0.86f), 0f, 0.06f, new Vector2(40f, 40f));

            // Road — real asphalt texture. The plane is 70m long × 8m wide so
            // we tile ~1 repeat per 4m which keeps the grain crisp.
            var road = CreateCube("Road", env, new Vector3(0f, 0.01f, 0f),
                new Vector3(RoadLength, 0.02f, RoadWidth), RoadColor);
            ApplyTextured(road.GetComponent<MeshRenderer>(), _asphaltTex,
                new Color(0.85f, 0.85f, 0.88f), 0.06f, 0.55f,
                new Vector2(RoadLength / 4f, RoadWidth / 4f));

            // Sidewalks — concrete with panel seams.
            var sidewalkS = CreateCube("Sidewalk_South", env,
                new Vector3(0f, 0.06f, -(RoadWidth / 2f + SidewalkWidth / 2f)),
                new Vector3(RoadLength, 0.12f, SidewalkWidth), SidewalkColor);
            ApplyTextured(sidewalkS.GetComponent<MeshRenderer>(), _concreteTex,
                new Color(0.92f, 0.92f, 0.92f), 0f, 0.10f,
                new Vector2(RoadLength / 2f, SidewalkWidth / 2f));
            var sidewalkN = CreateCube("Sidewalk_North", env,
                new Vector3(0f, 0.06f,  (RoadWidth / 2f + SidewalkWidth / 2f)),
                new Vector3(RoadLength, 0.12f, SidewalkWidth), SidewalkColor);
            ApplyTextured(sidewalkN.GetComponent<MeshRenderer>(), _concreteTex,
                new Color(0.92f, 0.92f, 0.92f), 0f, 0.10f,
                new Vector2(RoadLength / 2f, SidewalkWidth / 2f));

            // Path from sidewalk to home / from sidewalk to school
            CreateCube("Path_FromHome", env,
                new Vector3(0f, 0.05f, ((-(RoadWidth / 2f) - SidewalkWidth) + HomeZ) / 2f),
                new Vector3(PathWidth, 0.10f, Mathf.Abs(HomeZ + RoadWidth / 2f + SidewalkWidth)),
                SidewalkColor);
            CreateCube("Path_ToSchool", env,
                new Vector3(0f, 0.05f, ((RoadWidth / 2f) + SidewalkWidth + SchoolZ) / 2f),
                new Vector3(PathWidth, 0.10f, Mathf.Abs(SchoolZ - RoadWidth / 2f - SidewalkWidth)),
                SidewalkColor);

            // Lane markings — dashed center line + solid edge lines.
            // (Skip the section right around the crosswalk so lane stripes
            //  don't paint over the zebra.)
            BuildLaneMarkings(env);

            // Crosswalk — thick zebra stripes spanning the full road width.
            // Stripes are oriented across the road (long in Z, narrow in X) so the
            // pedestrian walks across them from south curb to north curb.
            BuildZebraStripes(env);

            // Floating collectible coins on the crosswalk — picked up as the
            // kid walks across and added live to the wallet via the HUD.
            BuildZebraCoins(env);

            var marker = new GameObject("CrosswalkCenter");
            marker.transform.SetParent(env, false);
            marker.transform.position = Vector3.zero;
            crosswalkCenter = marker.transform;

            // Crosswalk approach lines on the sidewalks (subtle "stop here" marks)
            StripCollider(CreateCube("Curb_Stop_South", env,
                new Vector3(0f, 0.13f, -(RoadWidth / 2f + 0.4f)),
                new Vector3(PathWidth, 0.02f, 0.20f), CrosswalkColor));
            StripCollider(CreateCube("Curb_Stop_North", env,
                new Vector3(0f, 0.13f,  (RoadWidth / 2f + 0.4f)),
                new Vector3(PathWidth, 0.02f, 0.20f), CrosswalkColor));

            // Home (south end)
            var homeRoot = new GameObject("Home").transform;
            homeRoot.SetParent(env, false);
            homeRoot.position = new Vector3(0f, 0f, HomeZ);

            // Welcome-mat patio in front of the home door + a small lawn either side.
            var matColor = new Color(0.60f, 0.40f, 0.25f);
            StripCollider(CreateCube("Home_WelcomeMat", env,
                new Vector3(0f, 0.05f, HomeZ + 2.8f),
                new Vector3(1.6f, 0.05f, 0.7f), matColor));
            var yardColor2 = new Color(0.58f, 0.78f, 0.45f);
            var leftYard = CreateCube("Home_Yard_W", env,
                new Vector3(-2.8f, 0.03f, HomeZ + 2.0f),
                new Vector3(2.5f, 0.04f, 4.0f), yardColor2);
            ApplyTextured(leftYard.GetComponent<MeshRenderer>(), _grassTex,
                new Color(0.95f, 1f, 0.86f), 0f, 0.06f, new Vector2(2.5f, 2f));
            StripCollider(leftYard);
            var rightYard = CreateCube("Home_Yard_E", env,
                new Vector3(2.8f, 0.03f, HomeZ + 2.0f),
                new Vector3(2.5f, 0.04f, 4.0f), yardColor2);
            ApplyTextured(rightYard.GetComponent<MeshRenderer>(), _grassTex,
                new Color(0.95f, 1f, 0.86f), 0f, 0.06f, new Vector2(2.5f, 2f));
            StripCollider(rightYard);

            var homeBody = CreateCube("Home_Body", homeRoot, new Vector3(0f, 1.5f, 0f),
                new Vector3(4.5f, 3f, 4.5f), HomeColor);
            ApplyTextured(homeBody.GetComponent<MeshRenderer>(), _plasterTex,
                HomeColor * 1.08f, 0f, 0.18f, new Vector2(2f, 1.5f));

            var homeRoofObj = CreateCube("Home_Roof", homeRoot, new Vector3(0f, 3.4f, 0f),
                new Vector3(5f, 0.9f, 5f), HomeRoofColor);
            homeRoofObj.localRotation = Quaternion.Euler(0f, 45f, 0f);
            ApplyPBR(homeRoofObj.GetComponent<MeshRenderer>(), HomeRoofColor, 0.05f, 0.30f);

            var homeDoorColor = new Color(0.35f, 0.20f, 0.10f);
            var homeDoor = CreateCube("Home_Door", homeRoot, new Vector3(0f, 0.9f, 2.26f),
                new Vector3(1.0f, 1.8f, 0.05f), homeDoorColor);
            ApplyPBR(homeDoor.GetComponent<MeshRenderer>(), homeDoorColor, 0.10f, 0.40f);
            // Brass-ish door knob.
            StripCollider(CreateCube("Home_DoorKnob", homeRoot, new Vector3(0.35f, 0.95f, 2.30f),
                new Vector3(0.06f, 0.06f, 0.06f), new Color(0.85f, 0.65f, 0.20f)));

            MakeWindow(homeRoot, new Vector3(-1.2f, 1.8f, 2.26f),
                new Vector3(0.8f, 0.8f, 0.05f), lit: true);
            MakeWindow(homeRoot, new Vector3( 1.2f, 1.8f, 2.26f),
                new Vector3(0.8f, 0.8f, 0.05f), lit: false);
            MakeWindow(homeRoot, new Vector3( 2.26f, 1.8f, 0f),
                new Vector3(0.05f, 0.8f, 0.8f), lit: true);
            MakeWindow(homeRoot, new Vector3(-2.26f, 1.8f, 0f),
                new Vector3(0.05f, 0.8f, 0.8f), lit: false);

            // School (north end)
            var schoolRoot = new GameObject("School").transform;
            schoolRoot.SetParent(env, false);
            schoolRoot.position = new Vector3(0f, 0f, SchoolZ);

            var schoolBody = CreateCube("School_Body", schoolRoot, new Vector3(0f, 2.2f, 0f),
                new Vector3(8f, 4.4f, 5.5f), SchoolColor);
            ApplyTextured(schoolBody.GetComponent<MeshRenderer>(), _brickTex,
                SchoolColor * 1.05f, 0f, 0.15f, new Vector2(4f, 2.5f));

            var schoolRoof = CreateCube("School_Roof", schoolRoot, new Vector3(0f, 4.6f, 0f),
                new Vector3(8.4f, 0.4f, 5.9f), SchoolRoofColor);
            ApplyPBR(schoolRoof.GetComponent<MeshRenderer>(), SchoolRoofColor, 0.10f, 0.35f);

            // Lit "SCHOOL" sign — emissive so it bloomsproperly.
            var schoolSign = CreateCube("School_Sign", schoolRoot, new Vector3(0f, 3.7f, -2.8f),
                new Vector3(3.2f, 0.7f, 0.1f), Color.white);
            ApplyEmissive(schoolSign.GetComponent<MeshRenderer>(), Color.white, 1.5f);

            var schoolDoorColor = new Color(0.20f, 0.15f, 0.08f);
            var schoolDoor = CreateCube("School_Door", schoolRoot, new Vector3(0f, 1.0f, -2.76f),
                new Vector3(1.6f, 2.0f, 0.05f), schoolDoorColor);
            ApplyPBR(schoolDoor.GetComponent<MeshRenderer>(), schoolDoorColor, 0.10f, 0.40f);

            // Back door on the NORTH face of the school — the kid exits here on
            // her return trip home.
            var schoolBackDoor = CreateCube("School_BackDoor", schoolRoot,
                new Vector3(0f, 1.0f, 2.76f),
                new Vector3(1.6f, 2.0f, 0.05f), schoolDoorColor);
            ApplyPBR(schoolBackDoor.GetComponent<MeshRenderer>(), schoolDoorColor, 0.10f, 0.40f);
            StripCollider(schoolBackDoor);
            // Small "EXIT" sign over the back door.
            var backExitSign = CreateCube("School_BackDoorSign", schoolRoot,
                new Vector3(0f, 2.4f, 2.78f),
                new Vector3(1.2f, 0.35f, 0.06f), new Color(0.95f, 0.85f, 0.20f));
            ApplyEmissive(backExitSign.GetComponent<MeshRenderer>(),
                new Color(0.95f, 0.85f, 0.20f), 1.2f);
            StripCollider(backExitSign);

            MakeWindow(schoolRoot, new Vector3(-2.5f, 2.4f, -2.78f),
                new Vector3(1.0f, 1.0f, 0.04f), lit: true);
            MakeWindow(schoolRoot, new Vector3(-2.5f, 1.0f, -2.78f),
                new Vector3(1.0f, 1.0f, 0.04f), lit: false);
            MakeWindow(schoolRoot, new Vector3( 2.5f, 2.4f, -2.78f),
                new Vector3(1.0f, 1.0f, 0.04f), lit: true);
            MakeWindow(schoolRoot, new Vector3( 2.5f, 1.0f, -2.78f),
                new Vector3(1.0f, 1.0f, 0.04f), lit: true);
            // Side windows
            MakeWindow(schoolRoot, new Vector3(-4.04f, 2.4f, 0f),
                new Vector3(0.04f, 1.0f, 1.0f), lit: true);
            MakeWindow(schoolRoot, new Vector3(-4.04f, 2.4f, -1.5f),
                new Vector3(0.04f, 1.0f, 1.0f), lit: false);
            MakeWindow(schoolRoot, new Vector3( 4.04f, 2.4f, 0f),
                new Vector3(0.04f, 1.0f, 1.0f), lit: false);
            MakeWindow(schoolRoot, new Vector3( 4.04f, 2.4f, -1.5f),
                new Vector3(0.04f, 1.0f, 1.0f), lit: true);

            // Add a small ambient point light at the school entrance — makes
            // the door area "feel" lit even before the kid arrives.
            var entryLightGo = new GameObject("School_EntryLight");
            entryLightGo.transform.SetParent(schoolRoot, false);
            entryLightGo.transform.localPosition = new Vector3(0f, 2.8f, -3.2f);
            var entryLight = entryLightGo.AddComponent<Light>();
            entryLight.type = LightType.Point;
            entryLight.intensity = 1.8f;
            entryLight.range = 6f;
            entryLight.color = new Color(1.0f, 0.92f, 0.70f);
            entryLight.shadows = LightShadows.None;

            // Back-of-school CONCRETE COURTYARD + walkway. We build:
            //  • A wide patio behind the school (with a textured concrete look)
            //  • An L-shaped walkway: east from the back door, then south past
            //    the east face all the way down to the north sidewalk (no gaps,
            //    so the kid never has to step onto grass on her way home)
            //  • A picket fence around the schoolyard
            //  • A couple of decorative bushes + a wooden bench
            //  • Yellow breadcrumb arrows painted on the path showing the way
            var pathColor   = new Color(0.72f, 0.70f, 0.66f);
            var yardColor   = new Color(0.58f, 0.78f, 0.45f);
            var fenceColor  = new Color(0.95f, 0.94f, 0.90f);
            float backY = 0.05f;

            // Wide back patio just outside the back door (3m × 4m).
            var patio = CreateCube("School_BackPatio", env,
                new Vector3(0f, backY, SchoolZ + 3.5f),
                new Vector3(4f, 0.1f, 1.6f), pathColor);
            ApplyPBR(patio.GetComponent<MeshRenderer>(), pathColor, 0.05f, 0.20f);
            StripCollider(patio);

            // Grass courtyard fills the rest of the back area.
            var yard = CreateCube("School_BackYard", env,
                new Vector3(0f, backY - 0.02f, SchoolZ + 5.5f),
                new Vector3(11f, 0.06f, 4.5f), yardColor);
            ApplyTextured(yard.GetComponent<MeshRenderer>(), _grassTex,
                new Color(0.92f, 1f, 0.86f), 0f, 0.06f, new Vector2(6f, 3f));
            StripCollider(yard);

            // 1) Strip going EAST from back door to the east-back corner.
            var backStrip = CreateCube("School_BackPath_East", env,
                new Vector3(2.75f, backY, SchoolZ + 4.5f),
                new Vector3(4.5f, 0.1f, 1.4f), pathColor);
            ApplyPBR(backStrip.GetComponent<MeshRenderer>(), pathColor, 0.05f, 0.20f);
            StripCollider(backStrip);
            // 2) Strip going SOUTH along the east face of the school all the
            //    way down to (and slightly overlapping) the north sidewalk so
            //    the kid never has to step onto grass.
            const float northSidewalkSouthEdge = 4f; // RoadWidth/2 + SidewalkWidth/2 = 5.5 ± 1.5 → south edge at z=4
            float southStripCenter = (SchoolZ + 4.5f + northSidewalkSouthEdge) * 0.5f;
            float southStripLen    = (SchoolZ + 4.5f) - northSidewalkSouthEdge;
            var southStrip = CreateCube("School_BackPath_South", env,
                new Vector3(5f, backY, southStripCenter),
                new Vector3(1.5f, 0.1f, southStripLen), pathColor);
            ApplyPBR(southStrip.GetComponent<MeshRenderer>(), pathColor, 0.05f, 0.20f);
            StripCollider(southStrip);

            // 3) Yellow breadcrumb chevrons painted on the walkway pointing
            //    south to the sidewalk. Strictly visual — strip colliders.
            var breadcrumbColor = new Color(1.0f, 0.85f, 0.20f);
            for (float bz = SchoolZ + 4f; bz > 9f; bz -= 2.0f)
            {
                var crumb = CreateCube("PathArrow", env,
                    new Vector3(5f, 0.11f, bz),
                    new Vector3(0.8f, 0.02f, 0.15f), breadcrumbColor);
                StripCollider(crumb);
                ApplyEmissive(crumb.GetComponent<MeshRenderer>(), breadcrumbColor, 0.6f);
            }

            // 4) A second yellow chevron line along the north sidewalk from
            //    the east-side path west to the crosswalk centre.
            for (float bx = 4.5f; bx > 0.5f; bx -= 1.5f)
            {
                var crumb = CreateCube("PathArrow", env,
                    new Vector3(bx, 0.13f, 5.5f),
                    new Vector3(0.15f, 0.02f, 0.8f), breadcrumbColor);
                StripCollider(crumb);
                ApplyEmissive(crumb.GetComponent<MeshRenderer>(), breadcrumbColor, 0.6f);
            }
            // 5) Continue south of the road too — across the south sidewalk
            //    onto the home path.
            for (float bz = -4.5f; bz > HomeZ + 6f; bz -= 3f)
            {
                var crumb = CreateCube("PathArrow", env,
                    new Vector3(0f, 0.13f, bz),
                    new Vector3(0.8f, 0.02f, 0.15f), breadcrumbColor);
                StripCollider(crumb);
                ApplyEmissive(crumb.GetComponent<MeshRenderer>(), breadcrumbColor, 0.6f);
            }

            // ── Picket fence around the courtyard ──
            // Back fence (along z = SchoolZ + 7.5, parallel to road).
            float fenceZ = SchoolZ + 7.7f;
            for (float fx = -5.5f; fx <= 5.5f; fx += 0.45f)
            {
                StripCollider(CreateCube("Picket", env,
                    new Vector3(fx, 0.45f, fenceZ),
                    new Vector3(0.08f, 0.9f, 0.05f), fenceColor));
            }
            // Two horizontal rails.
            StripCollider(CreateCube("Rail_Top", env,
                new Vector3(0f, 0.78f, fenceZ),
                new Vector3(11.2f, 0.06f, 0.05f), fenceColor));
            StripCollider(CreateCube("Rail_Bot", env,
                new Vector3(0f, 0.18f, fenceZ),
                new Vector3(11.2f, 0.06f, 0.05f), fenceColor));
            // Short return fences on each side (so the courtyard reads as enclosed).
            for (float fz = SchoolZ + 3.2f; fz <= SchoolZ + 7.5f; fz += 0.45f)
            {
                StripCollider(CreateCube("Picket_W", env,
                    new Vector3(-5.6f, 0.45f, fz),
                    new Vector3(0.05f, 0.9f, 0.08f), fenceColor));
                StripCollider(CreateCube("Picket_E", env,
                    new Vector3( 5.6f, 0.45f, fz),
                    new Vector3(0.05f, 0.9f, 0.08f), fenceColor));
            }

            // A couple of trees + a wooden bench for the courtyard vignette.
            BuildTree(env, new Vector3(-4.2f, 0f, SchoolZ + 6.3f));
            BuildTree(env, new Vector3( 4.2f, 0f, SchoolZ + 6.3f));

            var benchColor = new Color(0.36f, 0.24f, 0.16f);
            var benchSeat = CreateCube("Bench_Seat", env,
                new Vector3(-2.2f, 0.45f, SchoolZ + 6.4f),
                new Vector3(1.6f, 0.10f, 0.55f), benchColor);
            ApplyPBR(benchSeat.GetComponent<MeshRenderer>(), benchColor, 0.05f, 0.40f);
            StripCollider(CreateCube("Bench_Leg_L", env,
                new Vector3(-2.95f, 0.225f, SchoolZ + 6.4f),
                new Vector3(0.10f, 0.45f, 0.50f), benchColor));
            StripCollider(CreateCube("Bench_Leg_R", env,
                new Vector3(-1.45f, 0.225f, SchoolZ + 6.4f),
                new Vector3(0.10f, 0.45f, 0.50f), benchColor));
            StripCollider(CreateCube("Bench_Back", env,
                new Vector3(-2.2f, 0.78f, SchoolZ + 6.65f),
                new Vector3(1.6f, 0.55f, 0.06f), benchColor));

            // Some decorative trees along the sidewalks for a sense of scale.
            for (int i = -2; i <= 2; i++)
            {
                if (i == 0) continue;
                float x = i * 6f;
                BuildTree(env, new Vector3(x, 0f, -(RoadWidth / 2f + SidewalkWidth + 1.2f)));
                BuildTree(env, new Vector3(x, 0f,  (RoadWidth / 2f + SidewalkWidth + 1.2f)));
            }
        }

        private static void BuildLaneMarkings(Transform env)
        {
            // Solid white edge lines along both road edges (just inside the curb).
            const float edgeInset = 0.10f;
            StripCollider(CreateCube("EdgeLine_South", env,
                new Vector3(0f, 0.025f, -(RoadWidth / 2f - edgeInset)),
                new Vector3(RoadLength, 0.03f, 0.10f), CrosswalkColor));
            StripCollider(CreateCube("EdgeLine_North", env,
                new Vector3(0f, 0.025f,  (RoadWidth / 2f - edgeInset)),
                new Vector3(RoadLength, 0.03f, 0.10f), CrosswalkColor));

            // Dashed centerline — 1m dashes with 1.5m gaps, skip the 6m crosswalk zone.
            const float dashLength = 1.0f;
            const float gapLength  = 1.5f;
            const float crosswalkClearance = 3.5f;  // half the no-stripe window
            float halfRoad = RoadLength / 2f;
            float cursor = -halfRoad + 1f;
            int i = 0;
            while (cursor + dashLength < halfRoad)
            {
                float center = cursor + dashLength / 2f;
                if (Mathf.Abs(center) > crosswalkClearance)
                {
                    StripCollider(CreateCube($"CenterDash_{i++}", env,
                        new Vector3(center, 0.025f, 0f),
                        new Vector3(dashLength, 0.03f, 0.12f), CrosswalkColor));
                }
                cursor += dashLength + gapLength;
            }

            // Stop lines on each lane just before the crosswalk (white, perpendicular
            // to the road). Cars decelerate here when their light is red.
            const float stopOffsetX = 5.5f;
            StripCollider(CreateCube("StopLine_East", env,
                new Vector3(-stopOffsetX, 0.025f, -1.8f),
                new Vector3(0.20f, 0.03f, 3.0f), CrosswalkColor));
            StripCollider(CreateCube("StopLine_West", env,
                new Vector3( stopOffsetX, 0.025f,  1.8f),
                new Vector3(0.20f, 0.03f, 3.0f), CrosswalkColor));
        }

        private static void BuildZebraStripes(Transform env)
        {
            // Stripes are HORIZONTAL bars from the camera's POV: each stripe runs
            // along the road direction (long X axis), and stripes are stacked along
            // the pedestrian walking direction (Z) from south curb to north curb.
            //
            // 7 stripes × 0.55m thick + 6 gaps × 0.55m = 6.85m total Z span,
            // which fits inside the 8m road width (centred at z=0).
            const int stripeCount = 7;
            const float stripeThicknessZ = 0.55f;
            const float stripeGapZ       = 0.55f;
            const float stripeHeightY    = 0.06f;
            const float stripeLengthX    = 5.5f;     // crosswalk is 5.5m wide along the road
            float totalSpan = stripeCount * stripeThicknessZ + (stripeCount - 1) * stripeGapZ;
            float startZ = -totalSpan / 2f + stripeThicknessZ / 2f;
            for (int i = 0; i < stripeCount; i++)
            {
                float z = startZ + i * (stripeThicknessZ + stripeGapZ);
                var stripe = CreateCube($"Zebra_{i}", env,
                    new Vector3(0f, 0.03f, z),
                    new Vector3(stripeLengthX, stripeHeightY, stripeThicknessZ),
                    CrosswalkColor);
                StripCollider(stripe); // decorative — kid steps on, not into.
            }
        }

        private static void BuildZebraCoins(Transform env)
        {
            // 5 coins evenly spaced along the kid's walk direction (Z), hovering
            // at hip height so a 1.5m-tall kid sweeps through them while crossing.
            var root = new GameObject("ZebraCoins").transform;
            root.SetParent(env, false);

            const int coinCount = 5;
            const float startZ = -2.6f;
            const float endZ   =  2.6f;
            float spacing = (endZ - startZ) / (coinCount - 1);

            for (int i = 0; i < coinCount; i++)
            {
                float z = startZ + i * spacing;
                MakeCoin(root, new Vector3(0f, 1.0f, z));
            }
        }

        private static void MakeCoin(Transform parent, Vector3 worldPosition)
        {
            var go = new GameObject("Coin");
            go.transform.SetParent(parent, false);
            go.transform.position = worldPosition;

            // Visual root — child transform so the coin can spin independently
            // of the trigger collider, and a small pop-scale on collect.
            var visual = new GameObject("Visual").transform;
            visual.SetParent(go.transform, false);
            visual.localPosition = Vector3.zero;
            // Orient the disc as a vertical coin whose flat face points along
            // the camera's line of sight (+/- Z). Combined with the world-Y
            // spin in Coin.cs, this gives the classic "gold coin spinning in
            // mid-air" look — the disc reads as a circle most of the time and
            // flashes to an edge as it rotates.
            visual.localRotation = Quaternion.Euler(90f, 0f, 0f);

            var coinYellow      = new Color(1.00f, 0.85f, 0.18f);
            var coinYellowDeep  = new Color(0.85f, 0.65f, 0.05f);
            var coinHighlight   = new Color(1.00f, 0.95f, 0.55f);

            // Main disc body (cylinder; default axis is Y, with the Z tilt above
            // it now stands as a vertical disc).
            CreateCylinder("Body", visual,
                new Vector3(0f, 0f, 0f),
                new Vector3(0.55f, 0.06f, 0.55f), coinYellow);
            // Rim (slightly larger, deeper yellow).
            CreateCylinder("Rim", visual,
                new Vector3(0f, 0f, 0f),
                new Vector3(0.60f, 0.04f, 0.60f), coinYellowDeep);
            // Inner highlight — small disc inset for a "stamp" feel.
            CreateCylinder("Inner", visual,
                new Vector3(0f, 0.035f, 0f),
                new Vector3(0.36f, 0.02f, 0.36f), coinHighlight);
            // Star mark in the middle — two thin crossed cubes (decorative).
            CreateCube("StarH", visual,
                new Vector3(0f, 0.045f, 0f),
                new Vector3(0.22f, 0.02f, 0.06f), coinYellowDeep);
            CreateCube("StarV", visual,
                new Vector3(0f, 0.045f, 0f),
                new Vector3(0.06f, 0.02f, 0.22f), coinYellowDeep);

            // Trigger pickup volume — a bit bigger than the visual so it grabs
            // the player even on slight lateral offsets.
            var col = go.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.55f;

            var coin = go.AddComponent<Coin>();
            coin.Configure(coinValue: 1, visual: visual);
        }

        private static void BuildTree(Transform env, Vector3 position)
        {
            var root = new GameObject("Tree").transform;
            root.SetParent(env, false);
            root.position = position;

            // Deterministic per-position variation so trees aren't all identical.
            int seed = Mathf.RoundToInt(position.x * 17.3f + position.z * 29.7f);
            var rng = new System.Random(seed);
            float scale = 0.85f + (float)rng.NextDouble() * 0.45f;
            float yaw = (float)rng.NextDouble() * 360f;
            root.localRotation = Quaternion.Euler(0f, yaw, 0f);

            // Cylinder trunk (more organic than a cube).
            var trunkColor = new Color(0.36f + 0.04f * (float)rng.NextDouble(),
                                       0.22f, 0.14f);
            CreateCylinder("Trunk", root,
                new Vector3(0f, 0.85f * scale, 0f),
                new Vector3(0.28f * scale, 0.85f * scale, 0.28f * scale), trunkColor);

            // Multi-sphere foliage — 3-5 spheres with varied size/colour for a
            // "puffy crown" look that reads as a real tree, not a lollipop.
            int blobCount = 4;
            float crownY = 1.95f * scale;
            float greenBase = 0.42f + 0.10f * (float)rng.NextDouble();
            for (int i = 0; i < blobCount; i++)
            {
                float angle = (i / (float)blobCount) * Mathf.PI * 2f
                              + (float)rng.NextDouble() * 0.6f;
                float r = 0.50f * scale;
                Vector3 offset = new Vector3(Mathf.Cos(angle) * r,
                                             (float)rng.NextDouble() * 0.30f * scale,
                                             Mathf.Sin(angle) * r);
                float blobScale = (0.95f + (float)rng.NextDouble() * 0.30f) * scale * 1.1f;
                var col = new Color(
                    0.15f + 0.10f * (float)rng.NextDouble(),
                    greenBase + 0.10f * (float)rng.NextDouble(),
                    0.18f + 0.08f * (float)rng.NextDouble());
                CreateSphere($"Foliage_{i}", root,
                    new Vector3(0f, crownY, 0f) + offset,
                    new Vector3(blobScale, blobScale, blobScale), col);
            }

            // Top crown blob a touch higher and centered, slightly lighter color.
            CreateSphere("Foliage_Top", root,
                new Vector3(0f, crownY + 0.35f * scale, 0f),
                new Vector3(1.1f * scale, 1.0f * scale, 1.1f * scale),
                new Color(0.25f + 0.05f * (float)rng.NextDouble(),
                          greenBase + 0.12f, 0.22f));
        }

        // ── Traffic light ────────────────────────────────────────────────────

        private static void BuildTrafficLight(out TrafficLight3D component, out MeshRenderer head)
        {
            var root = new GameObject("TrafficLight");
            root.transform.position = new Vector3(
                (RoadWidth / 2f) + 1.5f,
                0f,
                -(RoadWidth / 2f) - 0.6f);

            // Pole (concrete-style base + metal mast).
            var pBase = CreateCube("Pole_Base", root.transform, new Vector3(0f, 0.5f, 0f),
                new Vector3(0.55f, 1.0f, 0.55f), PoleColor);
            ApplyPBR(pBase.GetComponent<MeshRenderer>(), PoleColor, 0.20f, 0.30f);
            var pMast = CreateCylinder("Pole_Mast", root.transform, new Vector3(0f, 2.75f, 0f),
                new Vector3(0.16f, 1.75f, 0.16f), PoleColor);
            ApplyPBR(pMast.GetComponent<MeshRenderer>(), PoleColor, 0.75f, 0.55f);
            // Arm overhanging the road, so the housing sits over the lane.
            var pArm = CreateCube("Pole_Arm", root.transform, new Vector3(-0.55f, 4.55f, 0f),
                new Vector3(1.1f, 0.10f, 0.10f), PoleColor);
            ApplyPBR(pArm.GetComponent<MeshRenderer>(), PoleColor, 0.75f, 0.55f);

            // ── Housing ─────────────────────────────────────────────────────
            // A tall, narrow box mounted under the arm with three round lens
            // sockets (red on top, yellow in the middle, green on the bottom).
            var housingColor = new Color(0.04f, 0.04f, 0.05f);
            var housingPos = new Vector3(-1.0f, 4.05f, 0f);
            var housing = new GameObject("Head_Housing");
            housing.transform.SetParent(root.transform, false);
            housing.transform.localPosition = housingPos;

            var housingBack = CreateCube("Back", housing.transform, new Vector3(0f, 0f, 0f),
                new Vector3(0.50f, 1.40f, 0.30f), housingColor);
            ApplyPBR(housingBack.GetComponent<MeshRenderer>(), housingColor, 0.6f, 0.50f);
            // Slight backplate behind the housing — common on real intersections.
            var backplateColor = new Color(0.10f, 0.10f, 0.11f);
            var backplate = CreateCube("Backplate", housing.transform, new Vector3(0f, 0f, -0.10f),
                new Vector3(0.78f, 1.65f, 0.04f), backplateColor);
            ApplyPBR(backplate.GetComponent<MeshRenderer>(), backplateColor, 0.4f, 0.45f);

            // Three bulb positions: top (red), middle (yellow), bottom (green).
            float bulbSpacing = 0.42f;
            float bulbRadius = 0.16f;
            var redLens    = MakeBulb(housing.transform, "Bulb_Red",
                new Vector3(0f, +bulbSpacing, 0.16f), bulbRadius, new Color(1f, 0.10f, 0.10f));
            var yellowLens = MakeBulb(housing.transform, "Bulb_Yellow",
                new Vector3(0f, 0f, 0.16f), bulbRadius, new Color(1f, 0.78f, 0.15f));
            var greenLens  = MakeBulb(housing.transform, "Bulb_Green",
                new Vector3(0f, -bulbSpacing, 0.16f), bulbRadius, new Color(0.10f, 1f, 0.20f));

            // Visors — thin angled hoods that stick out over each bulb to keep
            // glare off and to match the silhouette of a real traffic light.
            MakeBulbVisor(housing.transform, new Vector3(0f, bulbSpacing + 0.18f, 0.20f));
            MakeBulbVisor(housing.transform, new Vector3(0f, 0.18f,                  0.20f));
            MakeBulbVisor(housing.transform, new Vector3(0f, -bulbSpacing + 0.18f,   0.20f));

            // The "Head" is now just a tiny invisible-ish placeholder so the
            // legacy single-renderer fallback in TrafficLight3D still resolves.
            // We hide it behind the housing.
            var legacyHead = CreateCube("Head", root.transform,
                housingPos + new Vector3(0f, 0f, -0.16f),
                new Vector3(0.001f, 0.001f, 0.001f), Color.black);
            head = legacyHead.GetComponent<MeshRenderer>();

            component = root.AddComponent<TrafficLight3D>();
            component.ConfigureBulbs(redLens, yellowLens, greenLens);
        }

        // Single circular bulb (sphere) facing the camera, emissive so it glows.
        private static MeshRenderer MakeBulb(Transform parent, string name, Vector3 pos,
            float radius, Color color)
        {
            var bulb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb.name = name;
            Object.DestroyImmediate(bulb.GetComponent<Collider>());
            bulb.transform.SetParent(parent, false);
            bulb.transform.localPosition = pos;
            bulb.transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
            var mr = bulb.GetComponent<MeshRenderer>();
            ApplyEmissive(mr, color, 0.001f); // emission keyword on; intensity driven at runtime
            return mr;
        }

        private static void MakeBulbVisor(Transform parent, Vector3 hoodPos)
        {
            var visorColor = new Color(0.04f, 0.04f, 0.05f);
            var v = CreateCube("Visor", parent, hoodPos,
                new Vector3(0.48f, 0.04f, 0.16f), visorColor);
            ApplyPBR(v.GetComponent<MeshRenderer>(), visorColor, 0.5f, 0.45f);
        }

        // ── Vehicles (6 looping, mixed types, traffic-light-aware) ──────────

        private enum VehicleKind { Sedan, Bajaj, Minibus, Truck }

        private static CarMover[] BuildVehicles(TrafficLight3D light)
        {
            var carsRoot = new GameObject("Traffic").transform;
            var movers = new System.Collections.Generic.List<CarMover>();
            float loopLen = CarLoopEndX - CarLoopStartX;

            // Level 1: cars are mellow (~14-18 km/h). Plenty of time to react.
            // 6 cars per lane on the 44m loop = ~7.3m gap between cars,
            // so the player ALWAYS sees a continuous stream of traffic in
            // the camera's ~24m horizontal slice — no more "empty road"
            // perception. The phase offset is a per-car progress shift in
            // METERS (converted to seconds via /speed) so spacing is exactly
            // 44/6 m regardless of individual car speed.
            //
            // Eastbound lane (south, driveWest=false)
            const float laneSpacing = 44f / 6f;
            const float eastBaseSpeed = 4.0f;          // ~14 km/h
            movers.Add(MakeVehicle(carsRoot, "Sedan_East",   VehicleKind.Sedan,   CarColor1,                       false, eastBaseSpeed + 0.3f, -0f * laneSpacing / eastBaseSpeed, light));
            movers.Add(MakeVehicle(carsRoot, "Bajaj_East",   VehicleKind.Bajaj,   CarColor3,                       false, eastBaseSpeed,         -1f * laneSpacing / eastBaseSpeed, light));
            movers.Add(MakeVehicle(carsRoot, "Minibus_East", VehicleKind.Minibus, new Color(0.95f, 0.45f, 0.18f),  false, eastBaseSpeed + 0.1f, -2f * laneSpacing / eastBaseSpeed, light));
            movers.Add(MakeVehicle(carsRoot, "Truck_East",   VehicleKind.Truck,   CarColor6,                       false, eastBaseSpeed - 0.2f, -3f * laneSpacing / eastBaseSpeed, light));
            movers.Add(MakeVehicle(carsRoot, "Sedan2_East",  VehicleKind.Sedan,   new Color(0.85f, 0.20f, 0.20f),  false, eastBaseSpeed + 0.2f, -4f * laneSpacing / eastBaseSpeed, light));
            movers.Add(MakeVehicle(carsRoot, "Bajaj2_East",  VehicleKind.Bajaj,   new Color(0.20f, 0.55f, 0.85f),  false, eastBaseSpeed - 0.1f, -5f * laneSpacing / eastBaseSpeed, light));

            // Westbound lane (north, driveWest=true)
            const float westBaseSpeed = 4.0f;
            movers.Add(MakeVehicle(carsRoot, "Truck_West",   VehicleKind.Truck,   CarColor5,                       true,  westBaseSpeed - 0.2f, -0f * laneSpacing / westBaseSpeed, light));
            movers.Add(MakeVehicle(carsRoot, "Sedan_West",   VehicleKind.Sedan,   CarColor2,                       true,  westBaseSpeed + 0.3f, -1f * laneSpacing / westBaseSpeed, light));
            movers.Add(MakeVehicle(carsRoot, "Bajaj_West",   VehicleKind.Bajaj,   CarColor6,                       true,  westBaseSpeed,         -2f * laneSpacing / westBaseSpeed, light));
            movers.Add(MakeVehicle(carsRoot, "Minibus_West", VehicleKind.Minibus, CarColor4,                       true,  westBaseSpeed + 0.1f, -3f * laneSpacing / westBaseSpeed, light));
            movers.Add(MakeVehicle(carsRoot, "Sedan2_West",  VehicleKind.Sedan,   new Color(0.95f, 0.85f, 0.20f),  true,  westBaseSpeed + 0.2f, -4f * laneSpacing / westBaseSpeed, light));
            movers.Add(MakeVehicle(carsRoot, "Truck2_West",  VehicleKind.Truck,   new Color(0.30f, 0.30f, 0.30f),  true,  westBaseSpeed - 0.1f, -5f * laneSpacing / westBaseSpeed, light));

            return movers.ToArray();
        }

        // ── Parked / decorative vehicles ─────────────────────────────────────
        //
        // These have NO CarMover — they're stationary props that make the
        // street feel inhabited. Placed along the shoulders far from the
        // crosswalk so they don't visually interfere with traffic.

        private static void BuildParkedCars()
        {
            var root = new GameObject("ParkedCars").transform;
            // North shoulder — east end.
            var p1 = MakeParkedVehicle(root, "Parked_N1", VehicleKind.Sedan,
                new Color(0.35f, 0.45f, 0.65f),
                new Vector3(-22f, 0.02f, 3.2f), faceWest: true);
            _ = p1;
            // North shoulder — west end.
            MakeParkedVehicle(root, "Parked_N2", VehicleKind.Bajaj,
                new Color(0.85f, 0.70f, 0.25f),
                new Vector3(22f, 0.02f, 3.2f), faceWest: false);
            // South shoulder — east end.
            MakeParkedVehicle(root, "Parked_S1", VehicleKind.Minibus,
                new Color(0.20f, 0.50f, 0.30f),
                new Vector3(-26f, 0.02f, -3.2f), faceWest: false);
            // South shoulder — west end.
            MakeParkedVehicle(root, "Parked_S2", VehicleKind.Truck,
                new Color(0.55f, 0.25f, 0.20f),
                new Vector3(26f, 0.02f, -3.2f), faceWest: true);
        }

        private static GameObject MakeParkedVehicle(Transform parent, string name, VehicleKind kind,
            Color bodyColor, Vector3 worldPos, bool faceWest)
        {
            var root = new GameObject(name).transform;
            root.SetParent(parent, false);
            root.position = worldPos;
            root.rotation = Quaternion.LookRotation(faceWest ? Vector3.left : Vector3.right, Vector3.up);

            switch (kind)
            {
                case VehicleKind.Sedan:   BuildSedanBody(root, bodyColor);   break;
                case VehicleKind.Bajaj:   BuildBajajBody(root, bodyColor);   break;
                case VehicleKind.Minibus: BuildMinibusBody(root, bodyColor); break;
                case VehicleKind.Truck:   BuildTruckBody(root, bodyColor);   break;
            }
            return root.gameObject;
        }

        // ── Extra street props (bus stop, planters, trash bags) ──────────────
        //
        // The original BuildStreetProps() already places benches, a fire
        // hydrant, a mailbox and trash cans. This adds a SECOND layer of
        // bigger props (a full bus stop with a sign, planters at every
        // doorway, soft trash bags) so the street reads as a populated city
        // block rather than a clean prototype.

        private static void BuildExtraStreetProps()
        {
            var root = new GameObject("ExtraStreetProps").transform;

            // Bus stop near the east end of the south sidewalk.
            BuildBusStop(root, new Vector3(-18f, 0f, -(RoadWidth / 2f + SidewalkWidth / 2f + 0.1f)));

            // A pair of planters flanking the home doorway.
            BuildPlanter(root, new Vector3(-1.8f, 0f, HomeZ + 2.5f));
            BuildPlanter(root, new Vector3( 1.8f, 0f, HomeZ + 2.5f));
            // And two flanking the school front door.
            BuildPlanter(root, new Vector3(-1.6f, 0f, SchoolZ - 3.2f));
            BuildPlanter(root, new Vector3( 1.6f, 0f, SchoolZ - 3.2f));

            // A second hydrant + bagged trash on the OPPOSITE sidewalk from
            // the original fire-hydrant/trash-can pair so both sides look
            // lived-in.
            BuildHydrant(root, new Vector3(-12f, 0f,  (RoadWidth / 2f + SidewalkWidth / 2f)));
            BuildTrashBag(root, new Vector3( 14f, 0f,  (RoadWidth / 2f + SidewalkWidth / 2f - 0.4f)));
            BuildTrashBag(root, new Vector3( 14.7f, 0f, (RoadWidth / 2f + SidewalkWidth / 2f - 0.4f)));
        }

        private static void BuildBusStop(Transform parent, Vector3 pos)
        {
            var rootGo = new GameObject("BusStop");
            rootGo.transform.SetParent(parent, false);
            rootGo.transform.position = pos;

            var glass = new Color(0.45f, 0.55f, 0.62f);
            var frame = new Color(0.18f, 0.18f, 0.20f);
            var benchC = new Color(0.42f, 0.28f, 0.18f);

            // Shelter back wall + roof.
            StripCollider(CreateCube("Back",  rootGo.transform, new Vector3(0f, 1.10f, -0.55f),
                new Vector3(2.6f, 2.20f, 0.06f), glass));
            StripCollider(CreateCube("Roof",  rootGo.transform, new Vector3(0f, 2.25f, -0.20f),
                new Vector3(2.8f, 0.10f, 0.95f), frame));
            // Side panel.
            StripCollider(CreateCube("SideW", rootGo.transform, new Vector3(-1.30f, 1.10f, -0.10f),
                new Vector3(0.06f, 2.20f, 0.85f), glass));
            // Bench inside the shelter.
            StripCollider(CreateCube("BenchSeat", rootGo.transform, new Vector3(0f, 0.45f, -0.30f),
                new Vector3(2.2f, 0.10f, 0.40f), benchC));
            StripCollider(CreateCube("BenchLegL", rootGo.transform, new Vector3(-0.95f, 0.20f, -0.30f),
                new Vector3(0.08f, 0.40f, 0.35f), benchC));
            StripCollider(CreateCube("BenchLegR", rootGo.transform, new Vector3( 0.95f, 0.20f, -0.30f),
                new Vector3(0.08f, 0.40f, 0.35f), benchC));
            // Bus stop sign on a pole.
            StripCollider(CreateCube("Pole", rootGo.transform, new Vector3(1.45f, 1.05f, 0.20f),
                new Vector3(0.07f, 2.10f, 0.07f), frame));
            var sign = CreateCube("Sign", rootGo.transform, new Vector3(1.45f, 2.10f, 0.20f),
                new Vector3(0.55f, 0.40f, 0.05f), new Color(0.20f, 0.55f, 0.90f));
            ApplyEmissive(sign.GetComponent<MeshRenderer>(), new Color(0.20f, 0.55f, 0.90f), 0.8f);
            StripCollider(sign);
        }

        private static void BuildPlanter(Transform parent, Vector3 pos)
        {
            var rootGo = new GameObject("Planter");
            rootGo.transform.SetParent(parent, false);
            rootGo.transform.position = pos;
            var soil = new Color(0.30f, 0.20f, 0.12f);
            var pot  = new Color(0.85f, 0.78f, 0.66f);
            StripCollider(CreateCube("Pot", rootGo.transform, new Vector3(0f, 0.25f, 0f),
                new Vector3(0.55f, 0.50f, 0.55f), pot));
            StripCollider(CreateCube("Soil", rootGo.transform, new Vector3(0f, 0.51f, 0f),
                new Vector3(0.50f, 0.04f, 0.50f), soil));
            // A small bush of green spheres.
            var rng = new System.Random((int)(pos.x * 73 + pos.z * 19));
            for (int i = 0; i < 5; i++)
            {
                var leaf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.DestroyImmediate(leaf.GetComponent<Collider>());
                leaf.transform.SetParent(rootGo.transform, false);
                float r = 0.18f + (float)rng.NextDouble() * 0.10f;
                leaf.transform.localPosition = new Vector3(
                    ((float)rng.NextDouble() - 0.5f) * 0.35f,
                    0.65f + (float)rng.NextDouble() * 0.20f,
                    ((float)rng.NextDouble() - 0.5f) * 0.35f);
                leaf.transform.localScale = new Vector3(r, r, r);
                ApplyURPColor(leaf.GetComponent<MeshRenderer>(),
                    new Color(0.30f + (float)rng.NextDouble() * 0.20f,
                              0.55f + (float)rng.NextDouble() * 0.20f, 0.22f));
            }
        }

        private static void BuildHydrant(Transform parent, Vector3 pos)
        {
            var rootGo = new GameObject("Hydrant");
            rootGo.transform.SetParent(parent, false);
            rootGo.transform.position = pos;
            var red = new Color(0.85f, 0.18f, 0.18f);
            StripCollider(CreateCube("Body", rootGo.transform, new Vector3(0f, 0.45f, 0f),
                new Vector3(0.32f, 0.90f, 0.32f), red));
            StripCollider(CreateCube("Cap", rootGo.transform, new Vector3(0f, 0.97f, 0f),
                new Vector3(0.30f, 0.10f, 0.30f), red));
            StripCollider(CreateCube("NozzleE", rootGo.transform, new Vector3(0.20f, 0.55f, 0f),
                new Vector3(0.12f, 0.12f, 0.08f), red));
            StripCollider(CreateCube("NozzleW", rootGo.transform, new Vector3(-0.20f, 0.55f, 0f),
                new Vector3(0.12f, 0.12f, 0.08f), red));
        }

        private static void BuildTrashBag(Transform parent, Vector3 pos)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "TrashBag";
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            go.transform.position = pos + new Vector3(0f, 0.30f, 0f);
            go.transform.localScale = new Vector3(0.55f, 0.50f, 0.55f);
            ApplyPBR(go.GetComponent<MeshRenderer>(), new Color(0.10f, 0.10f, 0.12f), 0f, 0.10f);
        }

        // ── Ambient audio ────────────────────────────────────────────────────

        private static void BuildAmbientAudio(KidPlayer kid)
        {
            var go = new GameObject("AmbientAudio");
            var amb = go.AddComponent<AmbientAudio>();
            amb.Configure(kid);
        }

        private static CarMover MakeVehicle(Transform parent, string name, VehicleKind kind,
            Color bodyColor, bool driveWest, float speed, float phaseSeconds, TrafficLight3D light)
        {
            float laneZ = driveWest ? NorthLaneZ : SouthLaneZ;
            var root = new GameObject(name).transform;
            root.SetParent(parent, false);

            switch (kind)
            {
                case VehicleKind.Sedan:   BuildSedanBody(root, bodyColor);   break;
                case VehicleKind.Bajaj:   BuildBajajBody(root, bodyColor);   break;
                case VehicleKind.Minibus: BuildMinibusBody(root, bodyColor); break;
                case VehicleKind.Truck:   BuildTruckBody(root, bodyColor);   break;
            }

            // ── Make the car a solid physical object ──────────────────────
            // Replace the small per-part BoxColliders that CreateCube() left
            // behind with one clean car-sized hull. The kid's CharacterController
            // blocks against any non-trigger collider (static or moving via
            // transform.position), so this alone is enough to stop her walking
            // through a parked or oncoming car — no Rigidbody required.
            //
            // NOTE: We intentionally do NOT add a Rigidbody. Adding a kinematic
            // Rigidbody changes how Unity processes transform.position changes
            // and interacts with the editor's interpolation pipeline, which in
            // earlier tests caused cars to appear stationary on play even
            // though _progress was advancing each frame.
            StripCollidersRecursive(root);
            float halfLengthX, halfWidthZ, height, centerY;
            switch (kind)
            {
                case VehicleKind.Truck:   halfLengthX = 2.4f; halfWidthZ = 1.05f; height = 1.9f; centerY = 0.95f; break;
                case VehicleKind.Minibus: halfLengthX = 2.2f; halfWidthZ = 0.95f; height = 1.8f; centerY = 0.90f; break;
                case VehicleKind.Bajaj:   halfLengthX = 1.2f; halfWidthZ = 0.55f; height = 1.5f; centerY = 0.75f; break;
                case VehicleKind.Sedan:
                default:                  halfLengthX = 1.9f; halfWidthZ = 0.85f; height = 1.4f; centerY = 0.70f; break;
            }
            var hull = root.gameObject.AddComponent<BoxCollider>();
            hull.center = new Vector3(0f, centerY, 0f);
            hull.size   = new Vector3(halfLengthX * 2f, height, halfWidthZ * 2f);

            // ── Exhaust trail behind the rear bumper ──────────────────────
            // DISABLED: Removed white exhaust trails per user request.
            // var exhaust = new GameObject("Exhaust");
            // exhaust.transform.SetParent(root, false);
            // float rearZSign = -1f;            // local rear is along -X for the body builders
            // exhaust.transform.localPosition = new Vector3(rearZSign * halfLengthX * 0.95f,
            //                                               0.35f, 0f);
            // var trail = exhaust.AddComponent<TrailRenderer>();
            // trail.time = 0.55f;
            // trail.minVertexDistance = 0.15f;
            // trail.startWidth = 0.35f;
            // trail.endWidth = 0.05f;
            // trail.startColor = new Color(1f, 1f, 1f, 0.55f);
            // trail.endColor   = new Color(0.85f, 0.85f, 0.85f, 0f);
            // trail.material = new Material(Shader.Find("Sprites/Default"));
            // trail.material.color = Color.white;
            // trail.numCornerVertices = 2;
            // trail.numCapVertices = 2;
            // trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            // trail.receiveShadows = false;

            var mover = root.gameObject.AddComponent<CarMover>();
            mover.Configure(
                xStart: CarLoopStartX, xEnd: CarLoopEndX,
                z: laneZ, y: 0.02f,
                west: driveWest,
                metersPerSecond: speed,
                phaseSeconds: phaseSeconds,
                light: light);
            return mover;
        }

        // Real car paint = mid metallic + very smooth so it picks up sky + lights.
        private const float CarPaintMetallic   = 0.55f;
        private const float CarPaintSmoothness = 0.88f;
        // Tinted glass: dielectric (no metalness), nearly mirror-smooth.
        private const float CarGlassMetallic   = 0.00f;
        private const float CarGlassSmoothness = 0.96f;
        // Rubber wheels = pure rough black.
        private const float WheelMetallic      = 0.00f;
        private const float WheelSmoothness    = 0.18f;

        private static readonly Color CarGlassColor = new Color(0.14f, 0.22f, 0.32f, 1f);
        private static readonly Color WheelColor    = new Color(0.06f, 0.06f, 0.07f);
        private static readonly Color HeadlightCol  = new Color(1.00f, 0.95f, 0.78f);
        private static readonly Color TaillightCol  = new Color(1.00f, 0.10f, 0.05f);

        // Helpers shared by every vehicle body.

        private static readonly Color BumperColor      = new Color(0.10f, 0.10f, 0.11f);
        private static readonly Color GrilleColor      = new Color(0.04f, 0.04f, 0.05f);
        private static readonly Color ChromeColor      = new Color(0.85f, 0.85f, 0.88f);
        private static readonly Color LicensePlateCol  = new Color(0.95f, 0.95f, 0.90f);

        private static void AddCarDetails(Transform root,
            float halfWidth, float frontZ, float backZ,
            float bodyY, float roofY, float bumperY,
            bool sideMirrors = true, bool grille = true, bool plates = true)
        {
            if (grille)
            {
                // Front grille — dark slats sunken just below the headlights.
                var grilleObj = CreateCube("Grille", root,
                    new Vector3(0f, bumperY + 0.18f, frontZ + 0.005f),
                    new Vector3(halfWidth * 1.20f, 0.18f, 0.02f), GrilleColor);
                ApplyPBR(grilleObj.GetComponent<MeshRenderer>(), GrilleColor, 0.65f, 0.55f);
                // Three vertical chrome accents on the grille.
                for (int i = -1; i <= 1; i++)
                {
                    var slat = CreateCube($"Grille_Slat_{i+1}", root,
                        new Vector3(i * halfWidth * 0.45f, bumperY + 0.18f, frontZ + 0.012f),
                        new Vector3(0.05f, 0.16f, 0.01f), ChromeColor);
                    ApplyPBR(slat.GetComponent<MeshRenderer>(), ChromeColor, 0.85f, 0.85f);
                }
            }

            // Front + rear bumpers.
            var fBump = CreateCube("Bumper_F", root,
                new Vector3(0f, bumperY, frontZ + 0.03f),
                new Vector3(halfWidth * 2.05f, 0.15f, 0.10f), BumperColor);
            ApplyPBR(fBump.GetComponent<MeshRenderer>(), BumperColor, 0.20f, 0.45f);
            var rBump = CreateCube("Bumper_R", root,
                new Vector3(0f, bumperY, backZ - 0.03f),
                new Vector3(halfWidth * 2.05f, 0.15f, 0.10f), BumperColor);
            ApplyPBR(rBump.GetComponent<MeshRenderer>(), BumperColor, 0.20f, 0.45f);

            if (plates)
            {
                // Front + rear license plates.
                var pF = CreateCube("Plate_F", root,
                    new Vector3(0f, bumperY, frontZ + 0.085f),
                    new Vector3(0.36f, 0.10f, 0.01f), LicensePlateCol);
                ApplyPBR(pF.GetComponent<MeshRenderer>(), LicensePlateCol, 0f, 0.40f);
                var pR = CreateCube("Plate_R", root,
                    new Vector3(0f, bumperY, backZ - 0.085f),
                    new Vector3(0.36f, 0.10f, 0.01f), LicensePlateCol);
                ApplyPBR(pR.GetComponent<MeshRenderer>(), LicensePlateCol, 0f, 0.40f);
            }

            if (sideMirrors)
            {
                // Side mirrors on each front-cabin corner.
                float mirrorZ = frontZ * 0.32f;
                float mirrorY = roofY - 0.18f;
                var mL = CreateCube("Mirror_L", root,
                    new Vector3(-halfWidth - 0.12f, mirrorY, mirrorZ),
                    new Vector3(0.10f, 0.10f, 0.18f), BumperColor);
                ApplyPBR(mL.GetComponent<MeshRenderer>(), BumperColor, 0.30f, 0.55f);
                var mR = CreateCube("Mirror_R", root,
                    new Vector3( halfWidth + 0.12f, mirrorY, mirrorZ),
                    new Vector3(0.10f, 0.10f, 0.18f), BumperColor);
                ApplyPBR(mR.GetComponent<MeshRenderer>(), BumperColor, 0.30f, 0.55f);
                // Tiny chrome handles next to each door.
                var hL = CreateCube("Handle_L", root,
                    new Vector3(-halfWidth - 0.005f, bodyY, mirrorZ - 0.30f),
                    new Vector3(0.02f, 0.05f, 0.18f), ChromeColor);
                ApplyPBR(hL.GetComponent<MeshRenderer>(), ChromeColor, 0.85f, 0.80f);
                var hR = CreateCube("Handle_R", root,
                    new Vector3( halfWidth + 0.005f, bodyY, mirrorZ - 0.30f),
                    new Vector3(0.02f, 0.05f, 0.18f), ChromeColor);
                ApplyPBR(hR.GetComponent<MeshRenderer>(), ChromeColor, 0.85f, 0.80f);
            }
        }

        // Adds a silver hubcap disc inside each wheel.
        private static void AddHubcaps(Transform root, float halfTrack, float wheelBase,
            float wheelDiameter, float wheelThickness)
        {
            float wy = wheelDiameter / 2f;
            float capR = wheelDiameter * 0.30f;
            float capInset = wheelThickness * 0.55f;
            MakeHubcap(root, "Hub_FL", new Vector3(-halfTrack + capInset, wy,  wheelBase), capR);
            MakeHubcap(root, "Hub_FR", new Vector3( halfTrack - capInset, wy,  wheelBase), capR);
            MakeHubcap(root, "Hub_RL", new Vector3(-halfTrack + capInset, wy, -wheelBase), capR);
            MakeHubcap(root, "Hub_RR", new Vector3( halfTrack - capInset, wy, -wheelBase), capR);
        }

        private static void MakeHubcap(Transform parent, string name, Vector3 pos, float radius)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = new Vector3(0.02f, radius * 2f, radius * 2f);
            ApplyPBR(go.GetComponent<MeshRenderer>(), ChromeColor, 0.95f, 0.90f);
        }

        private static void BuildSedanBody(Transform root, Color color)
        {
            // Lower body (hood + trunk area).
            var hood = CreateCube("Hood", root, new Vector3(0f, 0.70f, 0.85f),
                new Vector3(1.18f, 0.55f, 0.95f), color);
            ApplyPBR(hood.GetComponent<MeshRenderer>(), color, CarPaintMetallic, CarPaintSmoothness);
            var trunk = CreateCube("Trunk", root, new Vector3(0f, 0.70f, -0.95f),
                new Vector3(1.18f, 0.55f, 0.75f), color);
            ApplyPBR(trunk.GetComponent<MeshRenderer>(), color, CarPaintMetallic, CarPaintSmoothness);
            // Mid body (door area, taller).
            var mid = CreateCube("MidBody", root, new Vector3(0f, 0.85f, -0.15f),
                new Vector3(1.20f, 0.80f, 1.55f), color);
            ApplyPBR(mid.GetComponent<MeshRenderer>(), color, CarPaintMetallic, CarPaintSmoothness);
            // Cabin (greenhouse) — narrower than the body so glass lines pinch in.
            var cabin = CreateCube("Cabin", root, new Vector3(0f, 1.35f, -0.10f),
                new Vector3(1.02f, 0.50f, 1.30f), color * 0.78f);
            ApplyPBR(cabin.GetComponent<MeshRenderer>(), color * 0.78f, CarPaintMetallic, CarPaintSmoothness);

            // Slanted windshields built into the cabin silhouette.
            var winF = CreateCube("Windshield_F", root, new Vector3(0f, 1.40f, 0.56f),
                new Vector3(0.98f, 0.42f, 0.04f), CarGlassColor);
            ApplyPBR(winF.GetComponent<MeshRenderer>(), CarGlassColor, CarGlassMetallic, CarGlassSmoothness);
            var winR = CreateCube("Windshield_R", root, new Vector3(0f, 1.40f, -0.76f),
                new Vector3(0.98f, 0.42f, 0.04f), CarGlassColor);
            ApplyPBR(winR.GetComponent<MeshRenderer>(), CarGlassColor, CarGlassMetallic, CarGlassSmoothness);
            // Side glass panes.
            var sideL = CreateCube("SideGlass_L", root, new Vector3(-0.52f, 1.40f, -0.10f),
                new Vector3(0.02f, 0.40f, 1.10f), CarGlassColor);
            ApplyPBR(sideL.GetComponent<MeshRenderer>(), CarGlassColor, CarGlassMetallic, CarGlassSmoothness);
            var sideR = CreateCube("SideGlass_R", root, new Vector3( 0.52f, 1.40f, -0.10f),
                new Vector3(0.02f, 0.40f, 1.10f), CarGlassColor);
            ApplyPBR(sideR.GetComponent<MeshRenderer>(), CarGlassColor, CarGlassMetallic, CarGlassSmoothness);

            AddWheels(root, halfTrack: 0.58f, wheelBase: 0.95f);
            AddHubcaps(root, halfTrack: 0.58f, wheelBase: 0.95f, wheelDiameter: 0.45f, wheelThickness: 0.20f);
            AddHeadlights(root, halfWidth: 0.40f, frontZ: 1.31f);
            AddTaillights(root, halfWidth: 0.40f, backZ: -1.31f);
            AddCarDetails(root, halfWidth: 0.60f,
                frontZ: 1.32f, backZ: -1.32f,
                bodyY: 0.85f, roofY: 1.32f, bumperY: 0.55f);
        }

        private static void BuildBajajBody(Transform root, Color color)
        {
            // 3-wheel auto-rickshaw: narrow, short, single front wheel, two rear, tall roof.
            var body = CreateCube("Body", root, new Vector3(0f, 0.55f, 0f),
                new Vector3(0.9f, 0.45f, 1.7f), color);
            ApplyPBR(body.GetComponent<MeshRenderer>(), color, CarPaintMetallic, CarPaintSmoothness);

            var roof = CreateCube("Roof", root, new Vector3(0f, 1.15f, -0.10f),
                new Vector3(0.85f, 0.55f, 1.3f), color * 0.85f);
            ApplyPBR(roof.GetComponent<MeshRenderer>(), color * 0.85f, CarPaintMetallic, CarPaintSmoothness);

            var windshield = CreateCube("Windshield", root, new Vector3(0f, 1.05f, 0.65f),
                new Vector3(0.75f, 0.55f, 0.05f), CarGlassColor);
            ApplyPBR(windshield.GetComponent<MeshRenderer>(), CarGlassColor, CarGlassMetallic, CarGlassSmoothness);

            // Wheels — single front, two rear
            CreateWheel("WheelF",  root, new Vector3( 0f,    0.18f,  0.75f), 0.18f, 0.36f);
            CreateWheel("WheelRL", root, new Vector3(-0.45f, 0.18f, -0.65f), 0.18f, 0.36f);
            CreateWheel("WheelRR", root, new Vector3( 0.45f, 0.18f, -0.65f), 0.18f, 0.36f);

            AddHeadlights(root, halfWidth: 0f, frontZ: 0.86f, lightWidth: 0.20f);
            AddTaillights(root, halfWidth: 0.30f, backZ: -0.86f);
        }

        private static void BuildMinibusBody(Transform root, Color color)
        {
            // White minibus taxi — longer, taller, full-height cabin, blue stripe.
            var body = CreateCube("Body", root, new Vector3(0f, 1.05f, 0f),
                new Vector3(1.5f, 1.5f, 3.8f), color);
            ApplyPBR(body.GetComponent<MeshRenderer>(), color, CarPaintMetallic, CarPaintSmoothness);

            Color stripeCol = new Color(0.20f, 0.45f, 0.85f);
            var stripeR = CreateCube("BlueStripe_R", root, new Vector3( 0.76f, 1.05f, 0f),
                new Vector3(0.02f, 0.3f, 3.8f), stripeCol);
            ApplyPBR(stripeR.GetComponent<MeshRenderer>(), stripeCol, CarPaintMetallic, CarPaintSmoothness);
            var stripeL = CreateCube("BlueStripe_L", root, new Vector3(-0.76f, 1.05f, 0f),
                new Vector3(0.02f, 0.3f, 3.8f), stripeCol);
            ApplyPBR(stripeL.GetComponent<MeshRenderer>(), stripeCol, CarPaintMetallic, CarPaintSmoothness);

            var windF = CreateCube("Windshield_F", root, new Vector3(0f, 1.4f, 1.92f),
                new Vector3(1.4f, 0.8f, 0.05f), CarGlassColor);
            ApplyPBR(windF.GetComponent<MeshRenderer>(), CarGlassColor, CarGlassMetallic, CarGlassSmoothness);
            var windR = CreateCube("Windshield_R", root, new Vector3(0f, 1.4f, -1.92f),
                new Vector3(1.4f, 0.7f, 0.05f), CarGlassColor);
            ApplyPBR(windR.GetComponent<MeshRenderer>(), CarGlassColor, CarGlassMetallic, CarGlassSmoothness);

            AddWheels(root, halfTrack: 0.70f, wheelBase: 1.4f, wheelDiameter: 0.5f);
            AddHubcaps(root, halfTrack: 0.70f, wheelBase: 1.4f, wheelDiameter: 0.5f, wheelThickness: 0.20f);
            AddHeadlights(root, halfWidth: 0.55f, frontZ: 1.92f);
            AddTaillights(root, halfWidth: 0.55f, backZ: -1.92f);
            AddCarDetails(root, halfWidth: 0.75f,
                frontZ: 1.93f, backZ: -1.93f,
                bodyY: 1.05f, roofY: 1.65f, bumperY: 0.55f,
                sideMirrors: true, grille: false, plates: true);
        }

        private static void BuildTruckBody(Transform root, Color color)
        {
            // Small cargo truck — short cab in front, longer flatbed cargo box behind.
            var cab = CreateCube("Cab", root, new Vector3(0f, 0.95f, 0.95f),
                new Vector3(1.4f, 1.3f, 1.4f), color);
            ApplyPBR(cab.GetComponent<MeshRenderer>(), color, CarPaintMetallic, CarPaintSmoothness);

            var cabWin = CreateCube("CabWindow", root, new Vector3(0f, 1.30f, 1.66f),
                new Vector3(1.2f, 0.55f, 0.05f), CarGlassColor);
            ApplyPBR(cabWin.GetComponent<MeshRenderer>(), CarGlassColor, CarGlassMetallic, CarGlassSmoothness);

            Color cargoColor = color * 0.65f;
            var cargo = CreateCube("Cargo", root, new Vector3(0f, 1.10f, -0.95f),
                new Vector3(1.55f, 1.6f, 2.4f), cargoColor);
            ApplyPBR(cargo.GetComponent<MeshRenderer>(), cargoColor, 0.35f, 0.55f);

            AddWheels(root, halfTrack: 0.70f, wheelBase: 1.5f, wheelDiameter: 0.55f);
            AddHubcaps(root, halfTrack: 0.70f, wheelBase: 1.5f, wheelDiameter: 0.55f, wheelThickness: 0.20f);
            AddHeadlights(root, halfWidth: 0.50f, frontZ: 1.66f);
            AddTaillights(root, halfWidth: 0.65f, backZ: -2.16f);
            AddCarDetails(root, halfWidth: 0.72f,
                frontZ: 1.67f, backZ: -2.17f,
                bodyY: 0.95f, roofY: 1.55f, bumperY: 0.50f,
                sideMirrors: true, grille: true, plates: true);
        }

        private static Transform CreateWheel(string name, Transform root, Vector3 pos,
            float thickness, float diameter)
        {
            var t = CreateCube(name, root, pos,
                new Vector3(thickness, diameter, diameter), WheelColor);
            ApplyPBR(t.GetComponent<MeshRenderer>(), WheelColor, WheelMetallic, WheelSmoothness);
            return t;
        }

        private static void AddWheels(Transform root, float halfTrack, float wheelBase,
            float wheelDiameter = 0.45f, float wheelThickness = 0.20f)
        {
            float wy = wheelDiameter / 2f;
            CreateWheel("WheelFL", root, new Vector3(-halfTrack, wy,  wheelBase), wheelThickness, wheelDiameter);
            CreateWheel("WheelFR", root, new Vector3( halfTrack, wy,  wheelBase), wheelThickness, wheelDiameter);
            CreateWheel("WheelRL", root, new Vector3(-halfTrack, wy, -wheelBase), wheelThickness, wheelDiameter);
            CreateWheel("WheelRR", root, new Vector3( halfTrack, wy, -wheelBase), wheelThickness, wheelDiameter);
        }

        private static void AddHeadlights(Transform root, float halfWidth, float frontZ,
            float lightWidth = 0.20f, float lightHeight = 0.20f)
        {
            if (halfWidth <= 0.001f)
            {
                var single = CreateCube("Headlight", root, new Vector3(0f, 0.78f, frontZ),
                    new Vector3(lightWidth, lightHeight, 0.05f), HeadlightCol);
                ApplyEmissive(single.GetComponent<MeshRenderer>(), HeadlightCol, 3.0f);
                return;
            }
            var l = CreateCube("Headlight_L", root, new Vector3(-halfWidth, 0.78f, frontZ),
                new Vector3(lightWidth, lightHeight, 0.05f), HeadlightCol);
            ApplyEmissive(l.GetComponent<MeshRenderer>(), HeadlightCol, 3.0f);
            var r = CreateCube("Headlight_R", root, new Vector3( halfWidth, 0.78f, frontZ),
                new Vector3(lightWidth, lightHeight, 0.05f), HeadlightCol);
            ApplyEmissive(r.GetComponent<MeshRenderer>(), HeadlightCol, 3.0f);
        }

        private static void AddTaillights(Transform root, float halfWidth, float backZ)
        {
            var l = CreateCube("Taillight_L", root, new Vector3(-halfWidth, 0.78f, backZ),
                new Vector3(0.20f, 0.18f, 0.05f), TaillightCol);
            ApplyEmissive(l.GetComponent<MeshRenderer>(), TaillightCol, 4.0f);
            var r = CreateCube("Taillight_R", root, new Vector3( halfWidth, 0.78f, backZ),
                new Vector3(0.20f, 0.18f, 0.05f), TaillightCol);
            ApplyEmissive(r.GetComponent<MeshRenderer>(), TaillightCol, 4.0f);
        }

        // ── Player ───────────────────────────────────────────────────────────

        private static void BuildPlayer(out GameObject playerGo, out KidPlayer kid)
        {
            playerGo = new GameObject("Player");
            playerGo.tag = "Player";
            playerGo.transform.position = new Vector3(0f, 1.0f, HomeZ + 3f);
            playerGo.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            // All visuals are nested under a VisualRoot so the bob animation moves
            // them as a single unit. The CharacterController on the Player remains
            // pinned at world Y by gravity.
            var visualRoot = new GameObject("VisualRoot").transform;
            visualRoot.SetParent(playerGo.transform, false);
            visualRoot.localPosition = Vector3.zero;

            var playerAnim = BuildHumanoidVisual(visualRoot,
                bodyColor: PlayerColor,
                skinColor: PlayerHeadColor,
                hasBackpack: true,
                backpackColor: new Color(0.18f, 0.40f, 0.72f),
                hairColor: new Color(0.16f, 0.10f, 0.06f),
                pantsColor: new Color(0.32f, 0.40f, 0.66f));

            var cc = playerGo.AddComponent<CharacterController>();
            cc.height = 1.5f;
            cc.radius = 0.32f;
            cc.center = Vector3.zero;
            cc.stepOffset = 0.3f;
            cc.slopeLimit = 45f;

            // Personal-space presence so NPCs yield as the kid approaches.
            var kidPresence = playerGo.AddComponent<CharacterPresence>();
            kidPresence.SetRadius(0.4f);

            kid = playerGo.AddComponent<KidPlayer>();
            kid.ConfigureBody(visualRoot);
            kid.ConfigureLimbAnimator(playerAnim);
        }

        /// <summary>
        /// Builds a proper humanoid figure (torso, head, two arms, two legs)
        /// under the given parent. Adds a HumanoidLimbAnimator wired to the
        /// four limb pivots so callers can drive the walking animation.
        ///
        /// The character is positioned with the visual root pivot at the hip;
        /// the figure stands roughly 1.4 m tall (Y from -0.75 to +0.65).
        /// </summary>
        private static HumanoidLimbAnimator BuildHumanoidVisual(Transform parent,
            Color bodyColor, Color skinColor, bool hasBackpack, Color backpackColor,
            Color? hairColor = null, Color? pantsColor = null)
        {
            Color hair  = hairColor  ?? new Color(0.18f, 0.10f, 0.06f);
            Color pants = pantsColor ?? new Color(0.20f, 0.25f, 0.40f);
            Color shoeC = new Color(0.10f, 0.10f, 0.12f);

            // ── Torso (shirt) ───────────────────────────────────────────
            CreateCube("Torso", parent,
                new Vector3(0f, 0.20f, 0f),
                new Vector3(0.38f, 0.46f, 0.24f), bodyColor);
            CreateCube("Torso_Collar", parent,
                new Vector3(0f, 0.42f, 0f),
                new Vector3(0.30f, 0.06f, 0.20f), bodyColor * 0.85f);

            // ── Neck ────────────────────────────────────────────────────
            CreateCylinder("Neck", parent,
                new Vector3(0f, 0.48f, 0f),
                new Vector3(0.10f, 0.06f, 0.10f), skinColor);

            // ── Head ────────────────────────────────────────────────────
            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            Object.DestroyImmediate(head.GetComponent<Collider>());
            head.transform.SetParent(parent, false);
            head.transform.localPosition = new Vector3(0f, 0.66f, 0f);
            head.transform.localScale = new Vector3(0.30f, 0.34f, 0.30f);
            ApplyURPColor(head.GetComponent<MeshRenderer>(), skinColor);

            // Hair cap (slightly oversize sphere covering top + back of head)
            var hairGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hairGo.name = "Hair";
            Object.DestroyImmediate(hairGo.GetComponent<Collider>());
            hairGo.transform.SetParent(parent, false);
            hairGo.transform.localPosition = new Vector3(0f, 0.72f, -0.02f);
            hairGo.transform.localScale = new Vector3(0.34f, 0.22f, 0.34f);
            ApplyURPColor(hairGo.GetComponent<MeshRenderer>(), hair);

            // Eyes
            for (int i = -1; i <= 1; i += 2)
            {
                var eyeWhite = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                eyeWhite.name = "EyeWhite_" + (i == -1 ? "L" : "R");
                Object.DestroyImmediate(eyeWhite.GetComponent<Collider>());
                eyeWhite.transform.SetParent(parent, false);
                eyeWhite.transform.localPosition = new Vector3(i * 0.07f, 0.69f, 0.13f);
                eyeWhite.transform.localScale = new Vector3(0.07f, 0.07f, 0.04f);
                ApplyURPColor(eyeWhite.GetComponent<MeshRenderer>(), Color.white);

                var pupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pupil.name = "Pupil_" + (i == -1 ? "L" : "R");
                Object.DestroyImmediate(pupil.GetComponent<Collider>());
                pupil.transform.SetParent(parent, false);
                pupil.transform.localPosition = new Vector3(i * 0.07f, 0.69f, 0.155f);
                pupil.transform.localScale = new Vector3(0.035f, 0.035f, 0.02f);
                ApplyURPColor(pupil.GetComponent<MeshRenderer>(), new Color(0.05f, 0.05f, 0.10f));
            }

            // Smile — small dark line at the mouth
            CreateCube("Mouth", parent,
                new Vector3(0f, 0.60f, 0.145f),
                new Vector3(0.08f, 0.012f, 0.01f), new Color(0.40f, 0.20f, 0.18f));

            // ── Arms (pivot at the shoulder so they swing) ─────────────
            var lArmPivot = new GameObject("LArm_Pivot").transform;
            lArmPivot.SetParent(parent, false);
            lArmPivot.localPosition = new Vector3(-0.22f, 0.38f, 0f);
            BuildArm(lArmPivot, bodyColor, skinColor);

            var rArmPivot = new GameObject("RArm_Pivot").transform;
            rArmPivot.SetParent(parent, false);
            rArmPivot.localPosition = new Vector3( 0.22f, 0.38f, 0f);
            BuildArm(rArmPivot, bodyColor, skinColor);

            // ── Legs (pivot at the hip) ────────────────────────────────
            var lLegPivot = new GameObject("LLeg_Pivot").transform;
            lLegPivot.SetParent(parent, false);
            lLegPivot.localPosition = new Vector3(-0.09f, 0f, 0f);
            BuildLeg(lLegPivot, pants, shoeC);

            var rLegPivot = new GameObject("RLeg_Pivot").transform;
            rLegPivot.SetParent(parent, false);
            rLegPivot.localPosition = new Vector3( 0.09f, 0f, 0f);
            BuildLeg(rLegPivot, pants, shoeC);

            // ── Backpack ───────────────────────────────────────────────
            if (hasBackpack)
            {
                CreateCube("Backpack", parent,
                    new Vector3(0f, 0.22f, -0.18f),
                    new Vector3(0.30f, 0.38f, 0.14f), backpackColor);
                CreateCube("Backpack_Top", parent,
                    new Vector3(0f, 0.41f, -0.16f),
                    new Vector3(0.20f, 0.05f, 0.10f), backpackColor * 0.75f);
                // Straps over shoulders
                CreateCube("Strap_L", parent,
                    new Vector3(-0.13f, 0.32f, -0.04f),
                    new Vector3(0.05f, 0.30f, 0.05f), backpackColor * 0.55f);
                CreateCube("Strap_R", parent,
                    new Vector3( 0.13f, 0.32f, -0.04f),
                    new Vector3(0.05f, 0.30f, 0.05f), backpackColor * 0.55f);
            }

            // ── Limb animator ──────────────────────────────────────────
            var animator = parent.gameObject.AddComponent<HumanoidLimbAnimator>();
            animator.Configure(lArmPivot, rArmPivot, lLegPivot, rLegPivot);
            return animator;
        }

        private static void BuildArm(Transform pivot, Color shirtColor, Color skinColor)
        {
            // Upper arm (shirt sleeve)
            CreateCube("Upper", pivot,
                new Vector3(0f, -0.16f, 0f),
                new Vector3(0.10f, 0.32f, 0.10f), shirtColor);
            // Elbow
            CreateSphere("Elbow", pivot,
                new Vector3(0f, -0.32f, 0f),
                new Vector3(0.11f, 0.11f, 0.11f), shirtColor);
            // Forearm (skin)
            CreateCube("Forearm", pivot,
                new Vector3(0f, -0.46f, 0f),
                new Vector3(0.09f, 0.28f, 0.09f), skinColor);
            // Hand
            CreateSphere("Hand", pivot,
                new Vector3(0f, -0.62f, 0f),
                new Vector3(0.10f, 0.10f, 0.10f), skinColor);
        }

        private static void BuildLeg(Transform pivot, Color pantsColor, Color shoeColor)
        {
            // Upper leg
            CreateCube("Upper", pivot,
                new Vector3(0f, -0.21f, 0f),
                new Vector3(0.14f, 0.40f, 0.14f), pantsColor);
            // Knee
            CreateSphere("Knee", pivot,
                new Vector3(0f, -0.42f, 0f),
                new Vector3(0.13f, 0.13f, 0.13f), pantsColor);
            // Lower leg
            CreateCube("Lower", pivot,
                new Vector3(0f, -0.56f, 0f),
                new Vector3(0.12f, 0.28f, 0.12f), pantsColor);
            // Shoe
            CreateCube("Shoe", pivot,
                new Vector3(0f, -0.71f, 0.05f),
                new Vector3(0.14f, 0.08f, 0.24f), shoeColor);
        }

        // ── Follow camera ────────────────────────────────────────────────────

        private static void BuildFollowCamera(Transform target)
        {
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            // 65° FOV (was 50°) so the player sees ~24m of road at the curb
            // instead of ~10m. With 8 cars spread across the 44m loop, this
            // means 4-5 cars are on-screen at any moment instead of only 1-2.
            cam.fieldOfView = 65f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 250f;
            cam.allowHDR = true;
            cam.allowMSAA = true;
            camGo.AddComponent<AudioListener>();

            // Camera sits a bit higher and further back so the whole road
            // (both lanes) is clearly framed in front of the kid.
            var offset = new Vector3(0f, 6.5f, -8.5f);
            var look = new Vector3(0f, 0.9f, 1.5f);
            var follow = camGo.AddComponent<FollowCamera>();
            follow.Configure(target, offset, look);
            camGo.transform.position = target.position + offset;
            camGo.transform.LookAt(target.position + look);

#if UNITY_POST_PROCESSING_STACK_V2
            var ppLayer = camGo.AddComponent<PostProcessLayer>();
            ppLayer.volumeLayer = ~0; // listen to volumes on every layer
            ppLayer.volumeTrigger = camGo.transform;
            if (IsMobileBuildTarget())
            {
                // FXAA is ~4x cheaper than SMAA on mobile and good enough at
                // the resolutions phones render at.
                ppLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                ppLayer.fastApproximateAntialiasing.fastMode = true;
            }
            else
            {
                ppLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                ppLayer.subpixelMorphologicalAntialiasing.quality =
                    SubpixelMorphologicalAntialiasing.Quality.High;
            }
#endif
        }

        // ── NPC pedestrians (decorative) ─────────────────────────────────────

        private static void BuildNPCs()
        {
            var npcsRoot = new GameObject("NPCs").transform;
            float sidewalkS = -(RoadWidth / 2f + SidewalkWidth / 2f);
            float sidewalkN =  (RoadWidth / 2f + SidewalkWidth / 2f);

            // ── Sidewalk strollers ────────────────────────────────────────
            MakeNPC(npcsRoot, "NPC_South",
                new Vector3( 7f, 0.55f, sidewalkS),
                new Vector3(20f, 0.55f, sidewalkS),
                bodyColor: new Color(0.85f, 0.30f, 0.40f),
                headColor: new Color(0.65f, 0.42f, 0.30f),
                packColor: new Color(0.20f, 0.60f, 0.30f),
                speed: 1.2f);

            MakeNPC(npcsRoot, "NPC_North",
                new Vector3(-20f, 0.55f, sidewalkN),
                new Vector3( -7f, 0.55f, sidewalkN),
                bodyColor: new Color(0.20f, 0.45f, 0.85f),
                headColor: new Color(0.85f, 0.70f, 0.55f),
                packColor: new Color(0.80f, 0.30f, 0.30f),
                speed: 1.4f);

            // Two more sidewalk pacers walking in the opposite direction.
            MakeNPC(npcsRoot, "NPC_SouthW",
                new Vector3(-22f, 0.55f, sidewalkS - 0.4f),
                new Vector3(-10f, 0.55f, sidewalkS - 0.4f),
                bodyColor: new Color(0.20f, 0.65f, 0.55f),
                headColor: new Color(0.85f, 0.70f, 0.55f),
                packColor: new Color(0.95f, 0.85f, 0.30f),
                speed: 1.0f);
            MakeNPC(npcsRoot, "NPC_NorthE",
                new Vector3( 10f, 0.55f, sidewalkN - 0.4f),
                new Vector3( 24f, 0.55f, sidewalkN - 0.4f),
                bodyColor: new Color(0.75f, 0.65f, 0.20f),
                headColor: new Color(0.60f, 0.40f, 0.28f),
                packColor: new Color(0.25f, 0.45f, 0.85f),
                speed: 1.3f);

            // NPC walking the home-path back to home (gives the kid something
            // to follow visually on the first leg).
            MakeNPC(npcsRoot, "NPC_HomePath",
                new Vector3(-1.2f, 0.55f, HomeZ + 4f),
                new Vector3(-1.2f, 0.55f, sidewalkS - 0.4f),
                bodyColor: new Color(0.55f, 0.30f, 0.75f),
                headColor: new Color(0.85f, 0.70f, 0.55f),
                packColor: new Color(0.20f, 0.20f, 0.20f),
                speed: 1.1f);

            // School-path NPC walking back from school (looks like another
            // kid heading home from school).
            MakeNPC(npcsRoot, "NPC_SchoolPath",
                new Vector3( 1.2f, 0.55f, SchoolZ - 5f),
                new Vector3( 1.2f, 0.55f, sidewalkN + 0.3f),
                bodyColor: new Color(0.30f, 0.55f, 0.30f),
                headColor: new Color(0.85f, 0.70f, 0.55f),
                packColor: new Color(0.20f, 0.40f, 0.65f),
                speed: 1.15f);

            // ── Idle "loiter" NPC at the bus stop ─────────────────────────
            // We give him a tiny back-and-forth path so he visibly shifts
            // weight without actually walking away.
            MakeNPC(npcsRoot, "NPC_BusStop",
                new Vector3(-18.3f, 0.55f, sidewalkS - 0.15f),
                new Vector3(-17.8f, 0.55f, sidewalkS - 0.15f),
                bodyColor: new Color(0.65f, 0.30f, 0.30f),
                headColor: new Color(0.85f, 0.70f, 0.55f),
                packColor: new Color(0.10f, 0.10f, 0.10f),
                speed: 0.4f);
        }

        private static void MakeNPC(Transform parent, string name, Vector3 a, Vector3 b,
            Color bodyColor, Color headColor, Color packColor, float speed)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = a;

            var visual = new GameObject("VisualRoot").transform;
            visual.SetParent(go.transform, false);
            visual.localPosition = Vector3.zero;
            var anim = BuildHumanoidVisual(visual, bodyColor, headColor, true, packColor);

            // Solid capsule so the player can't pass THROUGH the NPC. The kid's
            // CharacterController slides around it naturally.
            var col = go.AddComponent<CapsuleCollider>();
            col.height = 1.5f;
            col.radius = 0.35f;
            col.center = new Vector3(0f, 0.30f, 0f);

            var presence = go.AddComponent<CharacterPresence>();
            presence.SetRadius(0.4f);

            var npc = go.AddComponent<NPCWalker>();
            npc.Configure(a, b, speed, visual, anim);
        }

        // ── Crosswalk-using NPCs (demonstrate safe crossing) ─────────────────

        private static void BuildCrosswalkNPCs(TrafficLight3D light)
        {
            var root = new GameObject("CrosswalkNPCs").transform;

            // NPC A — adult in blue, paces between south-east sidewalk and
            // north-east sidewalk via the crosswalk. Slight +X lateral offset
            // so two NPCs don't perfectly overlap.
            MakeCrosswalkNPC(root, "NPC_Cross_A", light,
                bodyColor: new Color(0.20f, 0.40f, 0.78f),
                headColor: new Color(0.85f, 0.70f, 0.55f),
                packColor: new Color(0.95f, 0.85f, 0.20f),
                speed: 1.25f,
                BuildLoopPath(lateralOffset:  0.6f));

            // NPC B — woman in red dress, paces between south-west and
            // north-west sidewalk in the opposite phase. Negative lateral
            // offset so she stays on the opposite side of the crosswalk.
            MakeCrosswalkNPC(root, "NPC_Cross_B", light,
                bodyColor: new Color(0.78f, 0.25f, 0.30f),
                headColor: new Color(0.60f, 0.40f, 0.28f),
                packColor: new Color(0.20f, 0.55f, 0.30f),
                speed: 1.05f,
                BuildLoopPath(lateralOffset: -0.6f, reversed: true));
        }

        /// <summary>
        /// Returns a 4-waypoint closed loop that exits the south sidewalk,
        /// waits at the south curb, crosses to the north curb, walks along
        /// the north sidewalk, waits at the north curb, crosses back, etc.
        /// </summary>
        private static List<CrosswalkNPCWalker.Waypoint> BuildLoopPath(
            float lateralOffset, bool reversed = false)
        {
            const float curbZ = 4.6f;       // just outside the crosswalk's road edge
            const float sidewalkZ = 5.8f;   // mid-sidewalk
            const float loiterX = 10f;      // how far east/west the NPC strolls
            float x = lateralOffset;
            float y = 0.55f;

            // Six points: south sidewalk → south curb (wait) → north curb →
            // north sidewalk → north curb (wait) → south curb → loop.
            var pts = new List<CrosswalkNPCWalker.Waypoint>
            {
                new CrosswalkNPCWalker.Waypoint
                {
                    position = new Vector3(loiterX + x, y, -sidewalkZ),
                    pauseSeconds = 0.6f,
                },
                new CrosswalkNPCWalker.Waypoint
                {
                    position = new Vector3(x, y, -curbZ),
                    waitForCarsRed = true,
                    pauseSeconds = 0.3f,
                },
                new CrosswalkNPCWalker.Waypoint
                {
                    position = new Vector3(x, y, curbZ),
                    pauseSeconds = 0.3f,
                },
                new CrosswalkNPCWalker.Waypoint
                {
                    position = new Vector3(loiterX + x, y, sidewalkZ),
                    pauseSeconds = 0.8f,
                },
                new CrosswalkNPCWalker.Waypoint
                {
                    position = new Vector3(x, y, curbZ),
                    waitForCarsRed = true,
                    pauseSeconds = 0.3f,
                },
                new CrosswalkNPCWalker.Waypoint
                {
                    position = new Vector3(x, y, -curbZ),
                    pauseSeconds = 0.3f,
                },
            };

            if (reversed) pts.Reverse();
            return pts;
        }

        private static void MakeCrosswalkNPC(Transform parent, string name, TrafficLight3D light,
            Color bodyColor, Color headColor, Color packColor, float speed,
            List<CrosswalkNPCWalker.Waypoint> path)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var visual = new GameObject("VisualRoot").transform;
            visual.SetParent(go.transform, false);
            visual.localPosition = Vector3.zero;
            var anim = BuildHumanoidVisual(visual, bodyColor, headColor, true, packColor);

            var col = go.AddComponent<CapsuleCollider>();
            col.height = 1.5f;
            col.radius = 0.35f;
            col.center = new Vector3(0f, 0.30f, 0f);

            var presence = go.AddComponent<CharacterPresence>();
            presence.SetRadius(0.4f);

            var npc = go.AddComponent<CrosswalkNPCWalker>();
            npc.Configure(path, speed, light, visual, anim);
        }

        // ── Side buildings (urban context) ───────────────────────────────────

        private static void BuildSideBuildings()
        {
            var root = new GameObject("SideBuildings").transform;

            // Far side rows along the road, beyond the sidewalks.
            // X positions are spread along the road length; Z is just outside
            // the sidewalk so they don't overlap path triggers.
            float southZ = -(RoadWidth / 2f + SidewalkWidth + 4.0f);
            float northZ =  (RoadWidth / 2f + SidewalkWidth + 4.0f);

            // South side: a corner shop near the crosswalk, then varied houses.
            MakeShop(root,    new Vector3(-9f, 0f, southZ), new Color(0.85f, 0.50f, 0.25f), "Corner Shop");
            MakeHouse(root,   new Vector3(-20f, 0f, southZ), new Color(0.80f, 0.65f, 0.50f));
            MakeHouse(root,   new Vector3(  8f, 0f, southZ), new Color(0.90f, 0.85f, 0.65f));
            MakeBuilding(root,new Vector3( 19f, 0f, southZ), new Color(0.55f, 0.65f, 0.80f), 6.5f);

            // North side: a school neighbor and a couple of small buildings.
            MakeBuilding(root,new Vector3(-21f, 0f, northZ), new Color(0.70f, 0.80f, 0.55f), 5.5f);
            MakeHouse(root,   new Vector3( -9f, 0f, northZ), new Color(0.95f, 0.80f, 0.55f));
            MakeShop(root,    new Vector3(  9f, 0f, northZ), new Color(0.35f, 0.60f, 0.55f), "Cafe");
            MakeBuilding(root,new Vector3( 21f, 0f, northZ), new Color(0.85f, 0.55f, 0.55f), 7f);

            // Bushes / shrubs near every building entrance for some greenery.
            float bushSouthZ = southZ + 2.8f;   // closer to the sidewalk
            float bushNorthZ = northZ - 2.8f;
            foreach (var x in new float[] { -20f, -9f, 8f, 19f })
            {
                MakeBushCluster(root, new Vector3(x - 1.8f, 0f, bushSouthZ));
                MakeBushCluster(root, new Vector3(x + 1.8f, 0f, bushSouthZ));
            }
            foreach (var x in new float[] { -21f, -9f, 9f, 21f })
            {
                MakeBushCluster(root, new Vector3(x - 1.8f, 0f, bushNorthZ));
                MakeBushCluster(root, new Vector3(x + 1.8f, 0f, bushNorthZ));
            }
        }

        // A small cluster of leafy spheres at ground level.
        private static void MakeBushCluster(Transform parent, Vector3 pos)
        {
            var root = new GameObject("Bush").transform;
            root.SetParent(parent, false);
            root.position = pos;

            float clusterSeed = pos.x * 7.3f + pos.z * 11.7f;
            var leafBase = new Color(0.28f, 0.50f, 0.22f);
            int blobs = 3 + Mathf.RoundToInt(Mathf.Abs(Mathf.PerlinNoise(clusterSeed, 1f) * 2f));
            for (int i = 0; i < blobs; i++)
            {
                float ang = i * (Mathf.PI * 2f / blobs) + clusterSeed * 0.03f;
                float r = 0.35f + Mathf.PerlinNoise(clusterSeed + i, 0.5f) * 0.30f;
                float size = 0.55f + Mathf.PerlinNoise(clusterSeed + i, 1.7f) * 0.30f;
                float y = 0.20f + Mathf.PerlinNoise(clusterSeed + i, 3.0f) * 0.18f;
                Vector3 local = new Vector3(Mathf.Cos(ang) * r, y, Mathf.Sin(ang) * r);
                var blob = CreateSphere($"Leaf_{i}", root, local,
                    new Vector3(size, size * 0.85f, size),
                    leafBase + new Color(Mathf.PerlinNoise(clusterSeed + i, 2.0f) * 0.10f - 0.05f,
                                        Mathf.PerlinNoise(clusterSeed + i, 5.0f) * 0.15f,
                                        Mathf.PerlinNoise(clusterSeed + i, 7.0f) * 0.10f - 0.05f));
                ApplyPBR(blob.GetComponent<MeshRenderer>(),
                    leafBase + new Color(0.04f, 0.06f, 0.03f), 0f, 0.10f);
            }
        }

        // Glowing window color used everywhere — warm interior light + sky-blue
        // tint so windows look real-ish under any light.
        private static readonly Color WindowLitColor = new Color(0.95f, 0.86f, 0.55f);
        private static readonly Color WindowDarkColor = new Color(0.18f, 0.26f, 0.36f);

        private static void MakeHouse(Transform parent, Vector3 pos, Color wallColor)
        {
            var root = new GameObject("House").transform;
            root.SetParent(parent, false);
            root.position = pos;

            var wall = CreateCube("Wall", root, new Vector3(0f, 1.4f, 0f),
                new Vector3(4.5f, 2.8f, 4.5f), wallColor);
            ApplyTextured(wall.GetComponent<MeshRenderer>(), _plasterTex,
                wallColor * 1.05f, 0f, 0.20f, new Vector2(2f, 1.5f));

            var roofColor = new Color(0.45f, 0.20f, 0.15f);
            var roof = CreateCube("Roof", root, new Vector3(0f, 3.0f, 0f),
                new Vector3(5.0f, 0.6f, 5.0f), roofColor);
            ApplyPBR(roof.GetComponent<MeshRenderer>(), roofColor, 0.10f, 0.30f);

            var doorColor = new Color(0.30f, 0.20f, 0.10f);
            var door = CreateCube("Door", root, new Vector3(0f, 0.8f, 2.26f),
                new Vector3(0.9f, 1.6f, 0.05f), doorColor);
            ApplyPBR(door.GetComponent<MeshRenderer>(), doorColor, 0.05f, 0.45f);

            MakeWindow(root, new Vector3(-1.3f, 1.7f, 2.26f), new Vector3(0.8f, 0.7f, 0.04f), lit: true);
            MakeWindow(root, new Vector3( 1.3f, 1.7f, 2.26f), new Vector3(0.8f, 0.7f, 0.04f), lit: false);
            MakeWindow(root, new Vector3( 2.26f, 1.7f, 0f), new Vector3(0.04f, 0.7f, 0.8f), lit: true);
            MakeWindow(root, new Vector3(-2.26f, 1.7f, 0f), new Vector3(0.04f, 0.7f, 0.8f), lit: false);
        }

        private static void MakeShop(Transform parent, Vector3 pos, Color wallColor, string signText)
        {
            var root = new GameObject("Shop").transform;
            root.SetParent(parent, false);
            root.position = pos;

            var wall = CreateCube("Wall", root, new Vector3(0f, 1.5f, 0f),
                new Vector3(5.5f, 3.0f, 5.0f), wallColor);
            ApplyTextured(wall.GetComponent<MeshRenderer>(), _brickTex,
                wallColor * 1.10f, 0f, 0.15f, new Vector2(2f, 1.5f));

            var roofColor = new Color(0.30f, 0.30f, 0.32f);
            var roof = CreateCube("Roof", root, new Vector3(0f, 3.2f, 0f),
                new Vector3(6.0f, 0.4f, 5.4f), roofColor);
            ApplyPBR(roof.GetComponent<MeshRenderer>(), roofColor, 0.3f, 0.35f);

            var signColor = new Color(0.95f, 0.85f, 0.20f);
            var sign = CreateCube("Sign", root, new Vector3(0f, 2.6f, 2.52f),
                new Vector3(4.5f, 0.6f, 0.06f), signColor);
            ApplyEmissive(sign.GetComponent<MeshRenderer>(), signColor, 1.2f);

            // Large storefront glass — emissive a little so it picks up bloom.
            var win = CreateCube("Window", root, new Vector3(0f, 1.4f, 2.52f),
                new Vector3(3.5f, 1.4f, 0.04f), WindowDarkColor);
            ApplyPBR(win.GetComponent<MeshRenderer>(), WindowDarkColor, 0f, 0.96f);

            var awningColor = new Color(0.78f, 0.20f, 0.20f);
            var awning = CreateCube("Awning", root, new Vector3(0f, 2.0f, 2.7f),
                new Vector3(5.0f, 0.05f, 0.6f), awningColor);
            ApplyPBR(awning.GetComponent<MeshRenderer>(), awningColor, 0f, 0.35f);
            _ = signText;
        }

        private static void MakeWindow(Transform parent, Vector3 pos, Vector3 size, bool lit)
        {
            // Daytime scene — every window renders as dark reflective glass, no
            // emissive interior. The `lit` parameter is kept for back-compat in
            // case we add a day/night cycle later.
            _ = lit;
            Color c = WindowDarkColor;
            var w = CreateCube("Window", parent, pos, size, c);
            ApplyPBR(w.GetComponent<MeshRenderer>(), c, 0f, 0.94f);
            StripCollider(w);

            // Thin window frame around the pane (purely decorative — strip colliders).
            var frameColor = new Color(0.12f, 0.12f, 0.13f);
            float fz = pos.z;
            float fx = pos.x;
            float fy = pos.y;
            // Build a frame either on the +/-Z face or on the +/-X face based on
            // which dimension is thin.
            if (size.z < 0.1f)
            {
                StripCollider(CreateCube("Frame_T", parent, new Vector3(fx, fy + size.y * 0.5f, fz + 0.001f * Mathf.Sign(fz)),
                    new Vector3(size.x + 0.06f, 0.04f, size.z + 0.001f), frameColor));
                StripCollider(CreateCube("Frame_B", parent, new Vector3(fx, fy - size.y * 0.5f, fz + 0.001f * Mathf.Sign(fz)),
                    new Vector3(size.x + 0.06f, 0.04f, size.z + 0.001f), frameColor));
                StripCollider(CreateCube("Frame_L", parent, new Vector3(fx - size.x * 0.5f, fy, fz + 0.001f * Mathf.Sign(fz)),
                    new Vector3(0.04f, size.y, size.z + 0.001f), frameColor));
                StripCollider(CreateCube("Frame_R", parent, new Vector3(fx + size.x * 0.5f, fy, fz + 0.001f * Mathf.Sign(fz)),
                    new Vector3(0.04f, size.y, size.z + 0.001f), frameColor));
            }
            else if (size.x < 0.1f)
            {
                StripCollider(CreateCube("Frame_T", parent, new Vector3(fx + 0.001f * Mathf.Sign(fx), fy + size.y * 0.5f, fz),
                    new Vector3(size.x + 0.001f, 0.04f, size.z + 0.06f), frameColor));
                StripCollider(CreateCube("Frame_B", parent, new Vector3(fx + 0.001f * Mathf.Sign(fx), fy - size.y * 0.5f, fz),
                    new Vector3(size.x + 0.001f, 0.04f, size.z + 0.06f), frameColor));
                StripCollider(CreateCube("Frame_L", parent, new Vector3(fx + 0.001f * Mathf.Sign(fx), fy, fz - size.z * 0.5f),
                    new Vector3(size.x + 0.001f, size.y, 0.04f), frameColor));
                StripCollider(CreateCube("Frame_R", parent, new Vector3(fx + 0.001f * Mathf.Sign(fx), fy, fz + size.z * 0.5f),
                    new Vector3(size.x + 0.001f, size.y, 0.04f), frameColor));
            }
        }

        private static void MakeBuilding(Transform parent, Vector3 pos, Color wallColor, float height)
        {
            var root = new GameObject("Building").transform;
            root.SetParent(parent, false);
            root.position = pos;

            var wall = CreateCube("Wall", root, new Vector3(0f, height / 2f, 0f),
                new Vector3(4f, height, 4f), wallColor);
            // Alternate between brick and plaster for variety.
            bool useBrick = (Mathf.RoundToInt(pos.x) % 2) == 0;
            var tex = useBrick ? _brickTex : _plasterTex;
            float wallTilingY = Mathf.Max(1f, height / 2f);
            ApplyTextured(wall.GetComponent<MeshRenderer>(), tex,
                wallColor * 1.10f, 0f, 0.18f, new Vector2(2f, wallTilingY));

            // Window grid
            int windowRows = Mathf.Max(1, Mathf.FloorToInt(height / 1.4f));
            // Pseudo-random "lit" pattern keyed off the building position so
            // each building has a different but stable set of glowing windows.
            int seed = Mathf.RoundToInt(pos.x * 7f + pos.z * 11f);
            for (int row = 0; row < windowRows; row++)
            {
                float wy = 0.9f + row * 1.4f;
                if (wy + 0.5f > height) break;
                bool litL = ((seed + row * 3) & 1) == 0;
                bool litR = ((seed + row * 5) & 2) != 0;
                MakeWindow(root, new Vector3(-1.1f, wy, 2.02f),
                    new Vector3(0.7f, 0.6f, 0.04f), litL);
                MakeWindow(root, new Vector3( 1.1f, wy, 2.02f),
                    new Vector3(0.7f, 0.6f, 0.04f), litR);
                // Side windows too, on +X face.
                bool litS = ((seed + row * 7) & 1) == 0;
                MakeWindow(root, new Vector3(2.02f, wy, 0f),
                    new Vector3(0.04f, 0.6f, 0.7f), litS);
                MakeWindow(root, new Vector3(-2.02f, wy, 0f),
                    new Vector3(0.04f, 0.6f, 0.7f), !litS);
            }

            var roofColor = new Color(0.22f, 0.22f, 0.24f);
            var roofCap = CreateCube("Roof", root, new Vector3(0f, height + 0.15f, 0f),
                new Vector3(4.3f, 0.3f, 4.3f), roofColor);
            ApplyPBR(roofCap.GetComponent<MeshRenderer>(), roofColor, 0.4f, 0.25f);

            // Roof cornice — a thin lighter band right under the roof.
            var cornice = new Color(wallColor.r * 0.7f, wallColor.g * 0.7f, wallColor.b * 0.7f);
            CreateCube("Cornice", root, new Vector3(0f, height - 0.08f, 0f),
                new Vector3(4.15f, 0.16f, 4.15f), cornice);
        }

        // ── Lampposts ───────────────────────────────────────────────────────

        private static void BuildLampposts()
        {
            var root = new GameObject("Lampposts").transform;
            // Place lampposts on the outer edges of the sidewalks (away from the
            // road), every 8m along X, but skip a 6m window around the crosswalk.
            float[] xs = new float[] { -24f, -16f, -8f, 8f, 16f, 24f };
            float zSouth = -(RoadWidth / 2f + SidewalkWidth - 0.3f);
            float zNorth =  (RoadWidth / 2f + SidewalkWidth - 0.3f);
            foreach (var x in xs)
            {
                MakeLamppost(root, new Vector3(x, 0f, zSouth));
                MakeLamppost(root, new Vector3(x, 0f, zNorth));
            }
        }

        private static void MakeLamppost(Transform parent, Vector3 pos)
        {
            var root = new GameObject("Lamppost").transform;
            root.SetParent(parent, false);
            root.position = pos;
            // Use cylinder for the pole — looks much more realistic.
            var metalColor = new Color(0.22f, 0.22f, 0.24f);
            var baseObj = CreateCube("Base", root, new Vector3(0f, 0.1f, 0f),
                new Vector3(0.32f, 0.2f, 0.32f), new Color(0.18f, 0.18f, 0.20f));
            ApplyPBR(baseObj.GetComponent<MeshRenderer>(), new Color(0.18f, 0.18f, 0.20f), 0.6f, 0.45f);
            var pole = CreateCylinder("Pole", root, new Vector3(0f, 2.0f, 0f),
                new Vector3(0.10f, 2.0f, 0.10f), metalColor);
            ApplyPBR(pole.GetComponent<MeshRenderer>(), metalColor, 0.7f, 0.55f);
            var arm = CreateCube("Arm", root, new Vector3(0f, 4.0f, 0.30f),
                new Vector3(0.08f, 0.08f, 0.60f), metalColor);
            ApplyPBR(arm.GetComponent<MeshRenderer>(), metalColor, 0.7f, 0.55f);

            // Daytime lamp fixture — dark glass shade, no emission, no point
            // light. (Lampposts are off during the day.)
            var shadeColor = new Color(0.14f, 0.14f, 0.16f);
            var lamp = CreateCube("Lamp", root, new Vector3(0f, 3.95f, 0.65f),
                new Vector3(0.30f, 0.20f, 0.30f), shadeColor);
            ApplyPBR(lamp.GetComponent<MeshRenderer>(), shadeColor, 0.30f, 0.50f);
        }

        // ── Street signs (stop sign + pedestrian crossing sign) ─────────────

        private static void BuildStreetSigns()
        {
            var root = new GameObject("StreetSigns").transform;

            // Stop signs at both ends of the road, on the side facing oncoming
            // traffic, just before the crosswalk.
            MakeStopSign(root,
                new Vector3(-6.5f, 0f, -(RoadWidth / 2f + 0.6f)),
                facingY: 90f);   // faces east-bound (positive X) traffic
            MakeStopSign(root,
                new Vector3( 6.5f, 0f,  (RoadWidth / 2f + 0.6f)),
                facingY: 270f);  // faces west-bound (negative X) traffic

            // Pedestrian crossing signs on both sides of the crosswalk
            // (the yellow diamond with the pedestrian walking icon).
            MakeCrossingSign(root,
                new Vector3(-2.5f, 0f, -(RoadWidth / 2f + SidewalkWidth - 0.3f)));
            MakeCrossingSign(root,
                new Vector3( 2.5f, 0f,  (RoadWidth / 2f + SidewalkWidth - 0.3f)));

            // School-children-crossing warning triangles flanking the path that
            // approaches the school. They face SOUTH so they're readable as the
            // kid walks toward the school.
            MakeSchoolCrossingSign(root,
                new Vector3(-3.5f, 0f, SchoolZ - 12f), facingY: 180f);
            MakeSchoolCrossingSign(root,
                new Vector3( 3.5f, 0f, SchoolZ - 12f), facingY: 180f);
            // One more on the school sidewalk, closer to the entrance.
            MakeSchoolCrossingSign(root,
                new Vector3(-5.5f, 0f, SchoolZ - 4.5f), facingY: 90f);
        }

        /// <summary>
        /// Red triangle "school children crossing" warning sign. Three red bars
        /// form an equilateral outline around a white inner plate; two black
        /// stick-children silhouettes are mounted in front. Mounted on a slim
        /// cylinder pole.
        /// </summary>
        private static void MakeSchoolCrossingSign(Transform parent, Vector3 pos, float facingY)
        {
            var root = new GameObject("SchoolCrossingSign").transform;
            root.SetParent(parent, false);
            root.position = pos;
            root.localRotation = Quaternion.Euler(0f, facingY, 0f);

            var poleColor  = new Color(0.22f, 0.22f, 0.24f);
            var red        = new Color(0.86f, 0.10f, 0.10f);
            var white      = new Color(0.96f, 0.96f, 0.96f);
            var black      = new Color(0.05f, 0.05f, 0.05f);

            // Pole — taller than a normal sign so kids see it from afar.
            CreateCylinder("Pole", root, new Vector3(0f, 1.35f, 0f),
                new Vector3(0.07f, 1.35f, 0.07f), poleColor);

            // Sign centroid in world space (relative to root). The triangle is
            // built around this centroid at world (0, signY, 0+ offsets).
            const float signY = 2.85f;
            const float side  = 0.80f;          // triangle side length
            const float bar   = side * 0.10f;   // border bar thickness
            const float depth = 0.04f;

            // ── White inner plate (slightly recessed behind the red bars). ──
            // We approximate the triangular interior by stacking three
            // narrow rectangles whose widths shrink toward the apex.
            //   row 0 (bottom): widest
            //   row 1 (mid):    medium
            //   row 2 (top):    narrow, near apex
            const int rows = 6;
            float h = side * 0.866f;   // triangle height (sqrt(3)/2)
            float bottomY = signY - h / 3f;   // triangle bottom edge (centroid at signY)
            float strip = h / rows;
            for (int r = 0; r < rows; r++)
            {
                // At row index r, the strip is at y = bottomY + (r + 0.5) * strip.
                // Width at that y in an equilateral triangle: w = side * (1 - r/rows).
                float y = bottomY + (r + 0.5f) * strip;
                float w = side * (1f - (r + 0.5f) / rows);
                if (w < 0.04f) continue;
                CreateCube($"InnerStrip_{r}", root,
                    new Vector3(0f, y, 0.024f),
                    new Vector3(w, strip * 1.02f, depth * 0.6f), white);
            }

            // ── Red triangle outline (3 bars at 60° angles). ───────────────
            // Bottom side: horizontal across the bottom of the triangle.
            CreateCube("Border_Bottom", root,
                new Vector3(0f, bottomY, 0.05f),
                new Vector3(side + bar * 0.6f, bar, depth), red);

            // Left side: from bottom-left vertex to apex; rotated +60° on Z.
            // Midpoint sits at x = -side/4, y = bottomY + h/2.
            var leftBar = CreateCube("Border_Left", root,
                new Vector3(-side * 0.25f, bottomY + h * 0.5f, 0.05f),
                new Vector3(side + bar * 0.6f, bar, depth), red);
            leftBar.localRotation = Quaternion.Euler(0f, 0f, 60f);

            // Right side: from bottom-right vertex to apex; rotated -60° on Z.
            var rightBar = CreateCube("Border_Right", root,
                new Vector3(side * 0.25f, bottomY + h * 0.5f, 0.05f),
                new Vector3(side + bar * 0.6f, bar, depth), red);
            rightBar.localRotation = Quaternion.Euler(0f, 0f, -60f);

            // ── Two black silhouette children running. ─────────────────────
            // The taller "older child" is on the right (behind), the shorter
            // "younger child" is on the left (in front), holding hands.
            // Everything is positioned at z = +0.06 so it sits in front of the
            // white plate and red bars.
            const float artZ = 0.06f;
            BuildSignKidSilhouette(root, new Vector3( 0.07f, signY - 0.02f, artZ),
                scale: 1.00f, black);
            BuildSignKidSilhouette(root, new Vector3(-0.09f, signY - 0.05f, artZ),
                scale: 0.85f, black);
        }

        private static void BuildSignKidSilhouette(Transform parent, Vector3 center,
            float scale, Color color)
        {
            float s = scale;
            // Head (slightly oblong sphere)
            CreateSphere("Head", parent,
                center + new Vector3(0f, 0.18f * s, 0f),
                new Vector3(0.09f * s, 0.10f * s, 0.04f), color);
            // Torso (cube)
            CreateCube("Body", parent,
                center + new Vector3(0f, 0.04f * s, 0f),
                new Vector3(0.13f * s, 0.18f * s, 0.04f), color);
            // Front leg (kicking forward — angled cube)
            var frontLeg = CreateCube("LegF", parent,
                center + new Vector3(0.05f * s, -0.12f * s, 0f),
                new Vector3(0.05f * s, 0.16f * s, 0.04f), color);
            frontLeg.localRotation = Quaternion.Euler(0f, 0f, -20f);
            // Back leg (planted)
            var backLeg = CreateCube("LegB", parent,
                center + new Vector3(-0.04f * s, -0.13f * s, 0f),
                new Vector3(0.05f * s, 0.16f * s, 0.04f), color);
            backLeg.localRotation = Quaternion.Euler(0f, 0f, 15f);
            // Front arm
            var frontArm = CreateCube("ArmF", parent,
                center + new Vector3(0.07f * s, 0.04f * s, 0f),
                new Vector3(0.04f * s, 0.14f * s, 0.04f), color);
            frontArm.localRotation = Quaternion.Euler(0f, 0f, -45f);
            // Back arm
            var backArm = CreateCube("ArmB", parent,
                center + new Vector3(-0.07f * s, 0.04f * s, 0f),
                new Vector3(0.04f * s, 0.14f * s, 0.04f), color);
            backArm.localRotation = Quaternion.Euler(0f, 0f, 30f);
        }

        private static void MakeStopSign(Transform parent, Vector3 pos, float facingY)
        {
            var root = new GameObject("StopSign").transform;
            root.SetParent(parent, false);
            root.position = pos;
            root.localRotation = Quaternion.Euler(0f, facingY, 0f);

            CreateCylinder("Pole", root, new Vector3(0f, 1.3f, 0f),
                new Vector3(0.07f, 1.3f, 0.07f), new Color(0.20f, 0.20f, 0.22f));
            // Octagon-ish: use a slightly thicker disk-shape via scaled cylinder
            // rotated to face out. Cylinder with low scaleY = a disc.
            var sign = CreateCylinder("Face", root,
                new Vector3(0f, 2.55f, 0.02f),
                new Vector3(0.55f, 0.04f, 0.55f),
                new Color(0.85f, 0.10f, 0.10f));
            sign.localRotation = Quaternion.Euler(90f, 0f, 0f);
            // White "STOP" text proxy — a thin white rectangle across the face.
            CreateCube("Text", root, new Vector3(0f, 2.55f, 0.06f),
                new Vector3(0.55f, 0.13f, 0.02f), Color.white);
        }

        private static void MakeCrossingSign(Transform parent, Vector3 pos)
        {
            var root = new GameObject("CrossingSign").transform;
            root.SetParent(parent, false);
            root.position = pos;

            CreateCylinder("Pole", root, new Vector3(0f, 1.1f, 0f),
                new Vector3(0.06f, 1.1f, 0.06f), new Color(0.20f, 0.20f, 0.22f));
            // Yellow diamond — a rotated cube.
            var diamond = CreateCube("Diamond", root,
                new Vector3(0f, 2.25f, 0.02f),
                new Vector3(0.55f, 0.55f, 0.04f),
                new Color(0.95f, 0.78f, 0.10f));
            diamond.localRotation = Quaternion.Euler(0f, 0f, 45f);
            // Black border (slightly larger diamond behind)
            var border = CreateCube("Border", root,
                new Vector3(0f, 2.25f, 0.018f),
                new Vector3(0.60f, 0.60f, 0.02f),
                new Color(0.05f, 0.05f, 0.05f));
            border.localRotation = Quaternion.Euler(0f, 0f, 45f);
            // Pictogram (simple — body + legs hint)
            CreateCube("Icon_Body", root, new Vector3(0f, 2.30f, 0.06f),
                new Vector3(0.06f, 0.16f, 0.02f), new Color(0.05f, 0.05f, 0.05f));
            CreateCube("Icon_Leg_L", root, new Vector3(-0.04f, 2.13f, 0.06f),
                new Vector3(0.04f, 0.12f, 0.02f), new Color(0.05f, 0.05f, 0.05f));
            CreateCube("Icon_Leg_R", root, new Vector3( 0.04f, 2.13f, 0.06f),
                new Vector3(0.04f, 0.12f, 0.02f), new Color(0.05f, 0.05f, 0.05f));
            CreateSphere("Icon_Head", root, new Vector3(0f, 2.45f, 0.06f),
                new Vector3(0.08f, 0.08f, 0.04f), new Color(0.05f, 0.05f, 0.05f));
        }

        // ── Street props (benches, fire hydrant, mailbox, trash can) ────────

        private static void BuildStreetProps()
        {
            var root = new GameObject("StreetProps").transform;

            // Fire hydrant near the home side
            MakeFireHydrant(root, new Vector3(-3.5f, 0f, -(RoadWidth / 2f + 1.5f)));

            // Mailbox by the home path
            MakeMailbox(root, new Vector3(2.0f, 0f, -(RoadWidth / 2f + SidewalkWidth - 0.5f)));

            // Trash cans on both sides
            MakeTrashCan(root, new Vector3(-12f, 0f, -(RoadWidth / 2f + 1.5f)));
            MakeTrashCan(root, new Vector3( 12f, 0f,  (RoadWidth / 2f + 1.5f)));

            // Benches near the school
            MakeBench(root,
                new Vector3(-3.5f, 0f, SchoolZ - 7f),
                rotationY: 0f);
            MakeBench(root,
                new Vector3( 3.5f, 0f, SchoolZ - 7f),
                rotationY: 0f);
        }

        private static void MakeFireHydrant(Transform parent, Vector3 pos)
        {
            var root = new GameObject("FireHydrant").transform;
            root.SetParent(parent, false);
            root.position = pos;
            var red = new Color(0.82f, 0.18f, 0.15f);
            CreateCylinder("Base", root, new Vector3(0f, 0.10f, 0f),
                new Vector3(0.36f, 0.10f, 0.36f), red);
            CreateCylinder("Body", root, new Vector3(0f, 0.45f, 0f),
                new Vector3(0.28f, 0.35f, 0.28f), red);
            CreateSphere("Top",   root, new Vector3(0f, 0.85f, 0f),
                new Vector3(0.32f, 0.20f, 0.32f), red);
            // Side caps
            CreateCylinder("Cap_L", root, new Vector3(-0.18f, 0.50f, 0f),
                new Vector3(0.10f, 0.10f, 0.10f), red * 0.85f);
            CreateCylinder("Cap_R", root, new Vector3( 0.18f, 0.50f, 0f),
                new Vector3(0.10f, 0.10f, 0.10f), red * 0.85f);
        }

        private static void MakeMailbox(Transform parent, Vector3 pos)
        {
            var root = new GameObject("Mailbox").transform;
            root.SetParent(parent, false);
            root.position = pos;
            CreateCylinder("Pole", root, new Vector3(0f, 0.6f, 0f),
                new Vector3(0.06f, 0.6f, 0.06f), new Color(0.30f, 0.30f, 0.30f));
            var blue = new Color(0.15f, 0.30f, 0.65f);
            CreateCube("Box", root, new Vector3(0f, 1.4f, 0f),
                new Vector3(0.45f, 0.35f, 0.55f), blue);
            // Rounded top
            CreateSphere("Top", root, new Vector3(0f, 1.55f, 0f),
                new Vector3(0.45f, 0.18f, 0.55f), blue);
            // Door
            CreateCube("Door", root, new Vector3(0f, 1.35f, 0.28f),
                new Vector3(0.30f, 0.25f, 0.02f), blue * 0.75f);
        }

        private static void MakeTrashCan(Transform parent, Vector3 pos)
        {
            var root = new GameObject("TrashCan").transform;
            root.SetParent(parent, false);
            root.position = pos;
            var green = new Color(0.18f, 0.30f, 0.18f);
            CreateCylinder("Body", root, new Vector3(0f, 0.50f, 0f),
                new Vector3(0.40f, 0.50f, 0.40f), green);
            CreateCylinder("Lid", root, new Vector3(0f, 1.05f, 0f),
                new Vector3(0.43f, 0.05f, 0.43f), green * 0.7f);
            // Handle
            CreateCube("Handle", root, new Vector3(0f, 1.13f, 0f),
                new Vector3(0.20f, 0.04f, 0.06f), green * 0.5f);
        }

        private static void MakeBench(Transform parent, Vector3 pos, float rotationY)
        {
            var root = new GameObject("Bench").transform;
            root.SetParent(parent, false);
            root.position = pos;
            root.localRotation = Quaternion.Euler(0f, rotationY, 0f);
            var wood = new Color(0.45f, 0.28f, 0.18f);
            var metal = new Color(0.20f, 0.20f, 0.22f);
            // Legs
            CreateCube("Leg_L", root, new Vector3(-0.6f, 0.25f, 0f),
                new Vector3(0.06f, 0.50f, 0.40f), metal);
            CreateCube("Leg_R", root, new Vector3( 0.6f, 0.25f, 0f),
                new Vector3(0.06f, 0.50f, 0.40f), metal);
            // Seat planks
            CreateCube("Seat", root, new Vector3(0f, 0.50f, 0f),
                new Vector3(1.50f, 0.06f, 0.42f), wood);
            // Backrest
            CreateCube("Back", root, new Vector3(0f, 0.80f, -0.18f),
                new Vector3(1.50f, 0.40f, 0.05f), wood);
        }

        // ── Mission system ───────────────────────────────────────────────────

        private static void BuildMissionManager(out MissionController controller)
        {
            var go = new GameObject("MissionManager");
            controller = go.AddComponent<MissionController>();
        }

        // ── Crosswalk trigger zones ──────────────────────────────────────────

        private static CrosswalkZone BuildCrosswalkTriggers(
            MissionController controller, TrafficLight3D light, KidPlayer kid)
        {
            var root = new GameObject("CrosswalkZone");
            root.transform.position = Vector3.zero;
            var zone = root.AddComponent<CrosswalkZone>();
            zone.Configure(controller, light, kid);

            var curb = CreateTrigger(root.transform, "Trigger_Curb",
                new Vector3(0f, 0.5f, -(RoadWidth / 2f + 0.5f)),
                new Vector3(5f, 1f, 1.6f));
            curb.AddComponent<CrosswalkTriggerRelay>().Configure(zone, CrosswalkTriggerKind.Curb);

            var road = CreateTrigger(root.transform, "Trigger_Road",
                new Vector3(0f, 0.5f, 0f),
                new Vector3(5f, 1f, RoadWidth));
            road.AddComponent<CrosswalkTriggerRelay>().Configure(zone, CrosswalkTriggerKind.Road);

            var farside = CreateTrigger(root.transform, "Trigger_FarSide",
                new Vector3(0f, 0.5f,  (RoadWidth / 2f + 0.5f)),
                new Vector3(5f, 1f, 1.6f));
            farside.AddComponent<CrosswalkTriggerRelay>().Configure(zone, CrosswalkTriggerKind.FarSide);

            return zone;
        }

        private static GameObject CreateTrigger(Transform parent, string name, Vector3 pos, Vector3 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            var box = go.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = size;
            return go;
        }

        private static SchoolGoal BuildSchoolGoal(CrosswalkZone crosswalk)
        {
            var go = new GameObject("SchoolGoal");
            go.transform.position = new Vector3(0f, 1f, SchoolZ - 3.5f);
            var box = go.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(5f, 2f, 1.6f);
            var goal = go.AddComponent<SchoolGoal>();
            goal.Configure(crosswalk);
            return goal;
        }

        // ── Home goal (return trip) ──────────────────────────────────────────

        private static HomeGoal BuildHomeGoal(CrosswalkZone crosswalk)
        {
            var go = new GameObject("HomeGoal");
            // Place the trigger right at the home doorway. HomeZ is the home
            // building's center; the door faces +Z and is at z = HomeZ + 2.26,
            // so put the trigger a touch further south so the kid runs INTO it.
            go.transform.position = new Vector3(0f, 1f, HomeZ + 3f);
            var box = go.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(5f, 2f, 1.6f);
            var goal = go.AddComponent<HomeGoal>();
            goal.Configure(crosswalk);
            return goal;
        }

        // ── Round-trip controller (school day → return trip) ─────────────────

        private static RoundTripController BuildRoundTripController(
            SchoolGoal schoolGoal, HomeGoal homeGoal,
            CrosswalkZone crosswalk, KidPlayer kid, Transform classroomAnchor)
        {
            var go = new GameObject("RoundTripController");
            var ctl = go.AddComponent<RoundTripController>();
            ctl.Configure(schoolGoal, homeGoal, crosswalk, kid);
            ctl.SetClassroom(classroomAnchor);

            // Kid comes out the back-east corner of the school (school body
            // spans x = ±4 and z = SchoolZ ± 2.75; we drop her at x=+5, z=+5
            // beyond the back wall) facing south.
            const float backExitX = 5.0f;
            float backExitZ = SchoolZ + 5f;
            ctl.SetBackExit(
                worldPos: new Vector3(backExitX, 1.0f, backExitZ),
                schoolDurationSeconds: 3.5f);

            // Waypoints — SIDEWALK-ONLY route. The kid only ever touches the
            // back-of-school concrete walkway, the north sidewalk, the zebra
            // crossing, the south sidewalk, and the home approach path. She
            // never walks across grass.
            //
            // Layout (north is +Z):
            //   ┌─ School ──────────────────────────┐
            //   │              [Back door]          │
            //   │                                   │
            //   │      ◄── Back patio ──►           │
            //   │                          ▲        │
            //   │                          │  WP1   │  ◄ south on east strip
            //   └──────────────────────────┼────────┘
            //                              │
            //                          ───►│◄───  WP2 (north sidewalk @ x=5)
            //                              │
            //                  ◄── WP3 ───►│   westbound along sidewalk
            //                              │
            //   ════════ ZEBRA ═══ WP4 ═══ ZEBRA ════════ (crosses road)
            //                              │
            //                          WP5 (south sidewalk centre)
            //                              │
            //                              ▼
            //                          WP6 home doorway
            ctl.SetReturnPath(new[]
            {
                new Vector3(backExitX, 1f, 18f),    // south past east face of school
                new Vector3(backExitX, 1f, 5.5f),   // arrive on north sidewalk (z = sidewalk centre)
                new Vector3(0f,        1f, 5.5f),   // walk west to crosswalk centre, on sidewalk
                new Vector3(0f,        1f, -5.5f),  // cross the road; lands on south sidewalk
                new Vector3(0f,        1f, HomeZ + 3f) // straight south on the home path
            });
            return ctl;
        }

        // ── School interior — classroom diorama ──────────────────────────────
        //
        // Built off to the +X side of the main scene (around x=200) so it's
        // out of camera range during normal play. The kid + follow camera are
        // teleported there for ~5 seconds while the screen is black; this is
        // what the player sees fade in: a classroom with a teacher, a
        // blackboard with a safety tip, six classmates at desks, and the kid
        // at her own desk. A small popup over the blackboard delivers the
        // educational message.

        private static Transform BuildSchoolInterior()
        {
            const float Cx = 200f;     // far east — out of normal camera range
            const float Cz = 0f;
            const float Cy = 0f;

            var root = new GameObject("SchoolInterior").transform;
            root.position = new Vector3(Cx, Cy, Cz);

            // ── Floor ─────────────────────────────────────────────────────
            var floorColor = new Color(0.75f, 0.62f, 0.45f);   // wood
            var floor = CreateCube("Floor", root,
                new Vector3(0f, 0.05f, 0f),
                new Vector3(14f, 0.1f, 11f), floorColor);
            ApplyPBR(floor.GetComponent<MeshRenderer>(), floorColor, 0.0f, 0.35f);

            // ── Walls (4 sides) ────────────────────────────────────────────
            var wallColor = new Color(0.98f, 0.94f, 0.86f);    // off-white
            StripCollider(CreateCube("Wall_Back",  root,
                new Vector3(0f, 1.6f,  5.55f), new Vector3(14f, 3.2f, 0.1f), wallColor));
            StripCollider(CreateCube("Wall_Front", root,
                new Vector3(0f, 1.6f, -5.55f), new Vector3(14f, 3.2f, 0.1f), wallColor));
            StripCollider(CreateCube("Wall_Left",  root,
                new Vector3(-7.05f, 1.6f, 0f), new Vector3(0.1f, 3.2f, 11.2f), wallColor));
            StripCollider(CreateCube("Wall_Right", root,
                new Vector3( 7.05f, 1.6f, 0f), new Vector3(0.1f, 3.2f, 11.2f), wallColor));

            // ── Ceiling (subtle, mostly for ambient occlusion) ──────────────
            var ceilColor = new Color(0.93f, 0.92f, 0.90f);
            StripCollider(CreateCube("Ceiling", root,
                new Vector3(0f, 3.2f, 0f), new Vector3(14f, 0.1f, 11.2f), ceilColor));

            // ── Blackboard at the front of the room ───────────────────────
            var boardFrame = new Color(0.55f, 0.35f, 0.20f);
            var boardSurface = new Color(0.10f, 0.20f, 0.16f);
            StripCollider(CreateCube("BoardFrame", root,
                new Vector3(0f, 1.8f, -5.45f),
                new Vector3(5.2f, 1.6f, 0.12f), boardFrame));
            var board = CreateCube("Board", root,
                new Vector3(0f, 1.8f, -5.39f),
                new Vector3(4.9f, 1.4f, 0.05f), boardSurface);
            StripCollider(board);
            ApplyPBR(board.GetComponent<MeshRenderer>(), boardSurface, 0.05f, 0.10f);

            // Faux "chalk text" on the board — a few thin white bars suggesting
            // hand-written lines. Looks like writing without needing real glyphs.
            var chalkColor = new Color(0.95f, 0.96f, 0.92f);
            for (int row = 0; row < 3; row++)
            {
                float y = 2.20f - row * 0.32f;
                float w = 3.5f - row * 0.55f;
                float xOff = (row % 2 == 0) ? -0.2f : 0.3f;
                StripCollider(CreateCube("ChalkLine", root,
                    new Vector3(xOff, y, -5.36f),
                    new Vector3(w, 0.05f, 0.02f), chalkColor));
            }

            // ── Teacher's desk at the front ────────────────────────────────
            var deskWood = new Color(0.45f, 0.30f, 0.18f);
            var teacherDesk = CreateCube("TeacherDesk", root,
                new Vector3(0f, 0.55f, -3.8f),
                new Vector3(2.2f, 0.10f, 1.0f), deskWood);
            ApplyPBR(teacherDesk.GetComponent<MeshRenderer>(), deskWood, 0.05f, 0.30f);
            StripCollider(CreateCube("TeacherLegFL", root,
                new Vector3(-0.95f, 0.27f, -4.2f), new Vector3(0.08f, 0.55f, 0.08f), deskWood));
            StripCollider(CreateCube("TeacherLegFR", root,
                new Vector3( 0.95f, 0.27f, -4.2f), new Vector3(0.08f, 0.55f, 0.08f), deskWood));
            StripCollider(CreateCube("TeacherLegBL", root,
                new Vector3(-0.95f, 0.27f, -3.4f), new Vector3(0.08f, 0.55f, 0.08f), deskWood));
            StripCollider(CreateCube("TeacherLegBR", root,
                new Vector3( 0.95f, 0.27f, -3.4f), new Vector3(0.08f, 0.55f, 0.08f), deskWood));

            // ── Teacher standing in front of the board ────────────────────
            var teacherGo = new GameObject("Teacher");
            teacherGo.transform.SetParent(root, false);
            teacherGo.transform.localPosition = new Vector3(-2.4f, 0f, -3.0f);
            teacherGo.transform.localRotation = Quaternion.Euler(0f, 80f, 0f); // facing students
            var tVisual = new GameObject("VisualRoot").transform;
            tVisual.SetParent(teacherGo.transform, false);
            BuildHumanoidVisual(tVisual,
                bodyColor: new Color(0.65f, 0.20f, 0.30f),     // red dress
                skinColor: new Color(0.85f, 0.70f, 0.55f),
                hasBackpack: false,
                backpackColor: Color.gray,
                hairColor: new Color(0.20f, 0.12f, 0.06f),
                pantsColor: new Color(0.20f, 0.18f, 0.14f));

            // ── Student desks in a 3 × 2 grid + the kid's reserved desk ───
            // Layout: rows along Z, columns along X.
            //   Row 1 (closest to teacher, z=-2):  2 student desks + KID's desk
            //   Row 2 (z=0):                        3 student desks
            //   Row 3 (z=2):                        2 student desks
            BuildStudentDesk(root, new Vector3(-2.2f, 0f, -1.5f), studentNumber: 0);
            BuildStudentDesk(root, new Vector3( 0.0f, 0f, -1.5f), studentNumber: 1);
            BuildKidDeskOnly  (root, new Vector3( 2.2f, 0f, -1.5f)); // empty — the player's desk
            BuildStudentDesk(root, new Vector3(-2.2f, 0f,  0.5f), studentNumber: 2);
            BuildStudentDesk(root, new Vector3( 0.0f, 0f,  0.5f), studentNumber: 3);
            BuildStudentDesk(root, new Vector3( 2.2f, 0f,  0.5f), studentNumber: 4);
            BuildStudentDesk(root, new Vector3(-1.1f, 0f,  2.5f), studentNumber: 5);
            BuildStudentDesk(root, new Vector3( 1.1f, 0f,  2.5f), studentNumber: 6);

            // ── Soft warm overhead light ──────────────────────────────────
            var lightGo = new GameObject("RoomLight");
            lightGo.transform.SetParent(root, false);
            lightGo.transform.localPosition = new Vector3(0f, 3.0f, 0f);
            var lt = lightGo.AddComponent<Light>();
            lt.type = LightType.Point;
            lt.color = new Color(1.0f, 0.93f, 0.82f);
            lt.intensity = 1.6f;
            lt.range = 14f;
            lt.shadows = LightShadows.None;   // mobile-friendly

            // ── Anchor the kid is teleported TO (her desk position) ───────
            // She stands BESIDE her desk facing the board (i.e., facing -Z).
            var kidAnchor = new GameObject("KidClassroomAnchor").transform;
            kidAnchor.SetParent(root, false);
            kidAnchor.localPosition = new Vector3(2.2f, 1.0f, -1.0f);
            kidAnchor.localRotation = Quaternion.Euler(0f, 180f, 0f);  // face the board

            return kidAnchor;
        }

        // ── Single student desk + a seated student ───────────────────────────
        private static void BuildStudentDesk(Transform parent, Vector3 localPos, int studentNumber)
        {
            BuildDeskOnly(parent, localPos);

            // Student colour palette — deterministic by student number so each
            // desk has a consistent occupant.
            var rng = new System.Random(studentNumber * 13 + 7);
            Color body = Color.HSVToRGB((float)rng.NextDouble(), 0.55f, 0.85f);
            Color hair = new Color(
                0.15f + 0.20f * (float)rng.NextDouble(),
                0.10f + 0.15f * (float)rng.NextDouble(),
                0.08f + 0.05f * (float)rng.NextDouble());
            Color pants = new Color(
                0.25f + 0.20f * (float)rng.NextDouble(),
                0.25f + 0.15f * (float)rng.NextDouble(),
                0.45f + 0.20f * (float)rng.NextDouble());

            // Build a SEATED humanoid: legs are folded under the desk, the body
            // sits 0.45m off the floor. We do this by giving the visual root
            // a low local position; the procedural humanoid is otherwise normal.
            var student = new GameObject("Student_" + studentNumber);
            student.transform.SetParent(parent, false);
            student.transform.localPosition = localPos + new Vector3(0f, 0.05f, 0.05f);
            student.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // face the teacher (-Z = forward in our system)
            var visual = new GameObject("VisualRoot").transform;
            visual.SetParent(student.transform, false);
            visual.localPosition = new Vector3(0f, -0.18f, 0f); // seated: drop the body
            BuildHumanoidVisual(visual,
                bodyColor: body,
                skinColor: new Color(0.82f, 0.66f, 0.50f),
                hasBackpack: false,
                backpackColor: Color.gray,
                hairColor: hair,
                pantsColor: pants);
        }

        private static void BuildKidDeskOnly(Transform parent, Vector3 localPos)
        {
            BuildDeskOnly(parent, localPos);
            // A nameplate so the kid's desk reads as HER spot.
            var plate = CreateCube("Nameplate", parent,
                localPos + new Vector3(0f, 0.62f, -0.05f),
                new Vector3(0.5f, 0.08f, 0.02f),
                new Color(1f, 0.85f, 0.20f));
            StripCollider(plate);
            ApplyEmissive(plate.GetComponent<MeshRenderer>(), new Color(1f, 0.85f, 0.20f), 0.8f);
        }

        private static void BuildDeskOnly(Transform parent, Vector3 localPos)
        {
            var deskWood = new Color(0.62f, 0.45f, 0.28f);
            var top = CreateCube("DeskTop", parent,
                localPos + new Vector3(0f, 0.60f, 0f),
                new Vector3(1.0f, 0.05f, 0.55f), deskWood);
            ApplyPBR(top.GetComponent<MeshRenderer>(), deskWood, 0.05f, 0.30f);
            StripCollider(top);
            // Legs
            for (int sx = -1; sx <= 1; sx += 2)
                for (int sz = -1; sz <= 1; sz += 2)
                {
                    StripCollider(CreateCube("DeskLeg", parent,
                        localPos + new Vector3(sx * 0.42f, 0.30f, sz * 0.22f),
                        new Vector3(0.05f, 0.60f, 0.05f), deskWood));
                }
            // Chair (just a seat + back, no legs to keep it visually simple)
            var chairWood = new Color(0.45f, 0.30f, 0.18f);
            StripCollider(CreateCube("ChairSeat", parent,
                localPos + new Vector3(0f, 0.42f, 0.42f),
                new Vector3(0.55f, 0.05f, 0.45f), chairWood));
            StripCollider(CreateCube("ChairBack", parent,
                localPos + new Vector3(0f, 0.72f, 0.62f),
                new Vector3(0.55f, 0.55f, 0.04f), chairWood));
        }

        // ── Floating 3D arrow ────────────────────────────────────────────────

        private static ObjectiveArrow BuildObjectiveArrow(Transform follow)
        {
            var root = new GameObject("ObjectiveArrow");

            // "Body" container that gets rotated to point at the target.
            var arrow = new GameObject("Arrow").transform;
            arrow.SetParent(root.transform, false);

            // Build a stylized arrow: a long flat shaft + a wider triangular
            // tip. Both use an emissive yellow material so it glows above the
            // kid's head against any backdrop.
            var yellow = new Color(1.00f, 0.85f, 0.20f);

            var shaft = CreateCube("Shaft", arrow, new Vector3(0f, 0f, -0.20f),
                new Vector3(0.18f, 0.18f, 0.70f), yellow);
            Object.DestroyImmediate(shaft.GetComponent<Collider>());
            ApplyEmissive(shaft.GetComponent<MeshRenderer>(), yellow, 1.6f);

            // Tip = stretched cube rotated 45° to look like a chevron.
            var tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tip.name = "Tip";
            tip.transform.SetParent(arrow, false);
            tip.transform.localPosition = new Vector3(0f, 0f, 0.35f);
            tip.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            tip.transform.localScale = new Vector3(0.55f, 0.18f, 0.55f);
            Object.DestroyImmediate(tip.GetComponent<Collider>());
            ApplyURPColor(tip.GetComponent<MeshRenderer>(), yellow);
            ApplyEmissive(tip.GetComponent<MeshRenderer>(), yellow, 1.6f);

            // Subtle ring on the ground for extra visibility (a thin yellow ring
            // hovering just below the arrow head).
            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Ring";
            ring.transform.SetParent(root.transform, false);
            ring.transform.localPosition = new Vector3(0f, -0.25f, 0f);
            ring.transform.localScale = new Vector3(0.45f, 0.025f, 0.45f);
            Object.DestroyImmediate(ring.GetComponent<Collider>());
            ApplyURPColor(ring.GetComponent<MeshRenderer>(), yellow);
            ApplyEmissive(ring.GetComponent<MeshRenderer>(), yellow, 1.2f);

            var oa = root.AddComponent<ObjectiveArrow>();
            oa.Configure(follow, arrow);
            return oa;
        }

        // ── Invisible target anchors the arrow points at ─────────────────────

        private static (Transform front, Transform back, Transform home) BuildObjectiveTargets()
        {
            var parent = new GameObject("ObjectiveTargets").transform;
            var front = new GameObject("Target_SchoolFront").transform;
            front.SetParent(parent, false);
            front.position = new Vector3(0f, 1.2f, SchoolZ - 3.0f);

            var back = new GameObject("Target_SchoolBack").transform;
            back.SetParent(parent, false);
            back.position = new Vector3(0f, 1.2f, SchoolZ + 3.0f);

            var home = new GameObject("Target_Home").transform;
            home.SetParent(parent, false);
            home.position = new Vector3(0f, 1.2f, HomeZ + 2.5f);

            return (front, back, home);
        }

        // ── Car reporter ─────────────────────────────────────────────────────

        private static void BuildCarReporter(MissionController controller,
            Transform crosswalkCenter, CarMover[] cars)
        {
            var go = new GameObject("CarDistanceReporter");
            var r = go.AddComponent<CarDistanceReporter>();
            r.Configure(controller, crosswalkCenter, cars);
        }

        // ── HUD — Stitch "Safe Steps Addis" redesign ─────────────────────────

        private static void BuildHUD(MissionController controller, TrafficLight3D light,
            CrosswalkZone zone, SchoolGoal goal, RoundTripController roundTrip)
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            var canvasGo = new GameObject("HUDCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            // The top area is intentionally transparent — game world shows through.
            // Stitch design has floating elements over the scene, no opaque top bar.

            // ── Top-left: vertical 3-circle traffic light widget ──────────
            BuildTrafficLightWidget(canvasGo.transform,
                new Vector2(0.04f, 0.78f), new Vector2(0.22f, 0.97f),
                out Image redLight, out Image yellowLight, out Image greenLight);

            // ── Top-right: coin badge (pill with $ icon + number) ────────
            var coinsLabel = BuildCoinBadge(canvasGo.transform,
                new Vector2(0.62f, 0.92f), new Vector2(0.96f, 0.985f));

            // ── Mission status card (next to the traffic light) ──────────
            var statusText = BuildMissionCard(canvasGo.transform,
                new Vector2(0.26f, 0.78f), new Vector2(0.96f, 0.91f),
                out Text titleLabel);

            // ── Bottom action bar: WAIT (yellow) + CROSS (green) ─────────
            var waitBtn = MakeStitchPillButton(canvasGo.transform, "WaitButton", "STOP",
                new Vector2(0.06f, 0.05f), new Vector2(0.49f, 0.13f),
                UiYellow, UiYellowShadow, UiNavy);
            var crossBtn = MakeStitchPillButton(canvasGo.transform, "CrossButton", "GO",
                new Vector2(0.51f, 0.05f), new Vector2(0.94f, 0.13f),
                UiGreen, UiGreenShadow, Color.white);

            // ── Win overlay (hidden until mission complete) ──────────────
            var winOverlay = BuildWinOverlay(canvasGo.transform,
                out Text rTitle, out Text rScore, out Text rStars, out Text rCoins);

            // ── Failure overlay (hidden until accident) ──────────────────
            var failureOverlay = BuildFailureOverlay(canvasGo.transform,
                out Text fTitle, out Text fMsg, out Button retryBtn);

            // ── Objective banner (transient, slides in from the top) ──────
            var objBanner = BuildObjectiveBanner(canvasGo.transform, out Text objText);

            var hud = canvasGo.AddComponent<WalkHUD>();
            hud.Configure(controller, light, zone, goal,
                titleLabel, coinsLabel,
                statusText,
                redLight, yellowLight, greenLight,
                crossBtn, waitBtn,
                winOverlay, rTitle, rScore, rStars, rCoins,
                failureOverlay, fTitle, fMsg, retryBtn);
            // Wire the round-trip controller so the result panel only fires
            // when she arrives back HOME, not when she first reaches school.
            hud.ConfigureRoundTrip(roundTrip);
            hud.ConfigureObjectiveBanner(objBanner, objText);
        }

        // ── Objective banner: floating pill at the top of the screen ─────────

        private static GameObject BuildObjectiveBanner(Transform parent, out Text bannerText)
        {
            // Centered banner near the top, wide pill on Stitch UI.
            var pill = MakePanel(parent, "ObjectiveBanner",
                new Vector2(0.10f, 0.69f), new Vector2(0.90f, 0.76f),
                new Color(0.05f, 0.10f, 0.18f, 0.92f));
            ApplyRounded(pill.GetComponent<Image>(), 0.55f);

            var textGo = new GameObject("Label");
            textGo.transform.SetParent(pill.transform, false);
            var rt = textGo.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(24f, 6f);
            rt.offsetMax = new Vector2(-24f, -6f);

            bannerText = textGo.AddComponent<Text>();
            bannerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            bannerText.fontSize = 56;
            bannerText.color = new Color(1f, 0.95f, 0.85f);
            bannerText.alignment = TextAnchor.MiddleCenter;
            bannerText.text = "Walk to school safely!";

            // Start hidden — WalkHUD will fade it in/out on demand.
            pill.gameObject.SetActive(false);
            return pill.gameObject;
        }

        // ── Traffic-light widget (3 stacked bulbs in a dark pill) ────────────

        private static void BuildTrafficLightWidget(Transform parent,
            Vector2 anchorMin, Vector2 anchorMax,
            out Image redLight, out Image yellowLight, out Image greenLight)
        {
            // Drop-shadow (offset 8px down).
            var shadow = MakePanel(parent, "TrafficLight_Shadow",
                anchorMin, anchorMax, new Color(0f, 0f, 0f, 0.18f));
            shadow.offsetMin = new Vector2(0f, -10f);
            shadow.offsetMax = new Vector2(0f, -2f);
            ApplyRounded(shadow.GetComponent<Image>(), 0.6f);

            // Dark housing.
            var housing = MakePanel(parent, "TrafficLight", anchorMin, anchorMax, UiTLBackdrop);
            ApplyRounded(housing.GetComponent<Image>(), 0.6f);
            housing.GetComponent<Image>().raycastTarget = false;

            // 3 bulb circles stacked vertically, evenly spaced inside the housing.
            redLight    = MakeCircleBulb(housing, "Red",    0.74f, 0.94f, BulbRedOff);
            yellowLight = MakeCircleBulb(housing, "Yellow", 0.42f, 0.62f, BulbYellowOff);
            greenLight  = MakeCircleBulb(housing, "Green",  0.10f, 0.30f, BulbGreenOn);
        }

        // Visual "off" defaults (HUD overrides via WalkHUD.Update).
        private static readonly Color BulbRedOff    = new Color(0.30f, 0.08f, 0.08f);
        private static readonly Color BulbYellowOff = new Color(0.30f, 0.25f, 0.05f);
        private static readonly Color BulbGreenOn   = new Color(0.31f, 0.80f, 0.38f);

        private static Image MakeCircleBulb(RectTransform parent, string name,
            float yMin, float yMax, Color color)
        {
            // Centered horizontally, vertical band yMin..yMax.
            var img = MakeImage(parent, "Bulb_" + name,
                new Vector2(0.18f, yMin), new Vector2(0.82f, yMax), color);
            // Use built-in Knob.psd as a round sprite so the rectangle reads as a circle.
            var knob = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
            if (knob != null)
            {
                img.sprite = knob;
                img.type = Image.Type.Simple;
                img.preserveAspect = true;
            }
            return img;
        }

        // ── Coin badge (yellow pill with $ icon + number) ───────────────────

        private static Text BuildCoinBadge(Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            var shadow = MakePanel(parent, "CoinBadge_Shadow", anchorMin, anchorMax,
                new Color(0f, 0f, 0f, 0.18f));
            shadow.offsetMin = new Vector2(0f, -8f);
            shadow.offsetMax = new Vector2(0f, -2f);
            ApplyRounded(shadow.GetComponent<Image>(), 0.25f);

            var pill = MakePanel(parent, "CoinBadge", anchorMin, anchorMax, UiBluePale);
            ApplyRounded(pill.GetComponent<Image>(), 0.25f);
            pill.GetComponent<Image>().raycastTarget = false;

            // Coin icon — yellow circle with "$" letter.
            var icon = MakeImage(pill, "Coin_Icon",
                new Vector2(0.04f, 0.12f), new Vector2(0.32f, 0.88f), UiYellow);
            var knob = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
            if (knob != null) { icon.sprite = knob; icon.preserveAspect = true; }
            var dollar = MakeLabel(icon.rectTransform, "Dollar", "$",
                new Vector2(0f, 0f), new Vector2(1f, 1f), 56, TextAnchor.MiddleCenter, UiYellowShadow);
            dollar.fontStyle = FontStyle.Bold;
            dollar.raycastTarget = false;

            var label = MakeLabel(pill, "Coins", "0",
                new Vector2(0.36f, 0.10f), new Vector2(0.96f, 0.90f),
                52, TextAnchor.MiddleLeft, UiNavy);
            label.fontStyle = FontStyle.Bold;
            label.raycastTarget = false;
            return label;
        }

        // ── Mission card (white rounded, "MISSION" header + body copy) ─────

        private static Text BuildMissionCard(Transform parent, Vector2 anchorMin, Vector2 anchorMax,
            out Text titleLabel)
        {
            var shadow = MakePanel(parent, "MissionCard_Shadow", anchorMin, anchorMax,
                new Color(0f, 0f, 0f, 0.15f));
            shadow.offsetMin = new Vector2(0f, -10f);
            shadow.offsetMax = new Vector2(0f, -2f);
            ApplyRounded(shadow.GetComponent<Image>(), 0.7f);

            var card = MakePanel(parent, "MissionCard", anchorMin, anchorMax, UiCardWhite);
            ApplyRounded(card.GetComponent<Image>(), 0.7f);
            card.GetComponent<Image>().raycastTarget = false;

            titleLabel = MakeLabel(card, "MissionHeader", "MISSION",
                new Vector2(0.06f, 0.62f), new Vector2(0.96f, 0.92f),
                32, TextAnchor.MiddleLeft, UiNavySoft);
            titleLabel.fontStyle = FontStyle.Bold;
            titleLabel.raycastTarget = false;

            var body = MakeLabel(card, "Body", "Wait for the light to turn RED for cars,\nthen tap GO.",
                new Vector2(0.06f, 0.06f), new Vector2(0.96f, 0.62f),
                34, TextAnchor.UpperLeft, UiNavy);
            body.raycastTarget = false;
            return body;
        }

        // ── Stitch-style pill button (rounded clay with dark shadow underneath) ─

        private static Button MakeStitchPillButton(Transform parent, string name, string label,
            Vector2 anchorMin, Vector2 anchorMax, Color faceColor, Color shadowColor, Color textColor)
        {
            // SHADOW: sibling rect offset 10px down. NOT a child so it never blocks clicks.
            var shadow = new GameObject(name + "_Shadow");
            shadow.transform.SetParent(parent, false);
            var sRt = shadow.AddComponent<RectTransform>();
            sRt.anchorMin = anchorMin; sRt.anchorMax = anchorMax;
            sRt.offsetMin = new Vector2(0f, -14f);
            sRt.offsetMax = new Vector2(0f, -4f);
            var sImg = shadow.AddComponent<Image>();
            sImg.color = shadowColor;
            sImg.raycastTarget = false;
            ApplyRounded(sImg, 0.3f);

            // BUTTON: Image (raycast target) + Button on same GameObject. Label child non-raycast.
            var btnGo = new GameObject(name);
            btnGo.transform.SetParent(parent, false);
            var bRt = btnGo.AddComponent<RectTransform>();
            bRt.anchorMin = anchorMin; bRt.anchorMax = anchorMax;
            bRt.offsetMin = Vector2.zero; bRt.offsetMax = Vector2.zero;
            var bImg = btnGo.AddComponent<Image>();
            bImg.color = faceColor;
            bImg.raycastTarget = true;
            ApplyRounded(bImg, 0.3f);

            var lbl = MakeLabel(bRt, "Label", label,
                new Vector2(0f, 0f), new Vector2(1f, 1f), 60, TextAnchor.MiddleCenter, textColor);
            lbl.fontStyle = FontStyle.Bold;
            lbl.raycastTarget = false;

            var btn = btnGo.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor       = Color.white;
            colors.highlightedColor  = new Color(1.05f, 1.05f, 1.05f, 1f);
            colors.pressedColor      = new Color(0.85f, 0.85f, 0.85f, 1f);
            colors.selectedColor     = Color.white;
            colors.disabledColor     = new Color(0.6f, 0.6f, 0.6f, 0.8f);
            colors.fadeDuration      = 0.06f;
            btn.colors = colors;
            btn.targetGraphic = bImg;
            return btn;
        }

        // ── Win overlay (Stitch results screen) ────────────────────────────

        private static GameObject BuildWinOverlay(Transform parent,
            out Text titleText, out Text scoreText, out Text starsText, out Text coinsText)
        {
            var overlayRt = MakePanel(parent, "WinOverlay",
                new Vector2(0f, 0f), new Vector2(1f, 1f), UiOverlayDim);
            overlayRt.gameObject.SetActive(false);

            // Centered card filling 84% width × 80% height.
            var cardShadow = MakePanel(overlayRt, "Card_Shadow",
                new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.92f),
                new Color(0f, 0f, 0f, 0.22f));
            cardShadow.offsetMin = new Vector2(0f, -16f);
            cardShadow.offsetMax = new Vector2(0f, -4f);
            ApplyRounded(cardShadow.GetComponent<Image>(), 0.7f);

            var card = MakePanel(overlayRt, "Card",
                new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.92f), UiCardWhite);
            ApplyRounded(card.GetComponent<Image>(), 0.7f);

            // Green "AMAZING JOB!" pill at top.
            var headerPillShadow = MakePanel(card, "HeaderPill_Shadow",
                new Vector2(0.15f, 0.875f), new Vector2(0.85f, 0.95f),
                UiGreenShadow);
            headerPillShadow.offsetMin = new Vector2(0f, -8f);
            headerPillShadow.offsetMax = new Vector2(0f, -2f);
            ApplyRounded(headerPillShadow.GetComponent<Image>(), 0.25f);

            var headerPill = MakePanel(card, "HeaderPill",
                new Vector2(0.15f, 0.875f), new Vector2(0.85f, 0.95f), UiGreen);
            ApplyRounded(headerPill.GetComponent<Image>(), 0.25f);
            var headerTxt = MakeLabel(headerPill, "Label", "AMAZING JOB!",
                new Vector2(0f, 0f), new Vector2(1f, 1f), 44, TextAnchor.MiddleCenter, Color.white);
            headerTxt.fontStyle = FontStyle.Bold;
            headerTxt.raycastTarget = false;

            titleText = MakeLabel(card, "Title", "You reached school!",
                new Vector2(0.05f, 0.74f), new Vector2(0.95f, 0.86f),
                56, TextAnchor.MiddleCenter, UiGreenDeep);
            titleText.fontStyle = FontStyle.Bold;

            // 3 star chips (yellow circles with ★)
            BuildStarChip(card, new Vector2(0.18f, 0.56f), new Vector2(0.34f, 0.72f), 0.85f);
            starsText = BuildStarChip(card, new Vector2(0.42f, 0.54f), new Vector2(0.58f, 0.74f), 1.0f);
            BuildStarChip(card, new Vector2(0.66f, 0.56f), new Vector2(0.82f, 0.72f), 0.85f);

            // Pale-blue stats row: "Safety Score" + "Bonus Coins"
            var scoreTile = MakePanel(card, "ScoreTile",
                new Vector2(0.08f, 0.36f), new Vector2(0.48f, 0.52f), UiBluePale);
            ApplyRounded(scoreTile.GetComponent<Image>(), 0.5f);
            MakeLabel(scoreTile, "Hdr", "SAFETY SCORE",
                new Vector2(0f, 0.55f), new Vector2(1f, 0.92f), 24, TextAnchor.MiddleCenter, UiNavySoft)
                .fontStyle = FontStyle.Bold;
            scoreText = MakeLabel(scoreTile, "Val", "—",
                new Vector2(0f, 0.10f), new Vector2(1f, 0.58f), 48, TextAnchor.MiddleCenter, UiGreenDeep);
            scoreText.fontStyle = FontStyle.Bold;

            var coinsTile = MakePanel(card, "CoinsTile",
                new Vector2(0.52f, 0.36f), new Vector2(0.92f, 0.52f), UiBluePale);
            ApplyRounded(coinsTile.GetComponent<Image>(), 0.5f);
            MakeLabel(coinsTile, "Hdr", "BONUS COINS",
                new Vector2(0f, 0.55f), new Vector2(1f, 0.92f), 24, TextAnchor.MiddleCenter, UiNavySoft)
                .fontStyle = FontStyle.Bold;
            coinsText = MakeLabel(coinsTile, "Val", "+0",
                new Vector2(0f, 0.10f), new Vector2(1f, 0.58f), 48, TextAnchor.MiddleCenter, UiYellowShadow);
            coinsText.fontStyle = FontStyle.Bold;

            // Big yellow "Next Mission" CTA button.
            MakeStitchPillButton(card, "NextMission", "Next Mission",
                new Vector2(0.10f, 0.18f), new Vector2(0.90f, 0.30f),
                UiYellow, UiYellowShadow, UiNavy);

            // "Play Again" light pill — reloads scene like Retry.
            var playAgain = MakeStitchPillButton(card, "PlayAgain", "Play Again",
                new Vector2(0.22f, 0.05f), new Vector2(0.78f, 0.14f),
                UiBluePale, new Color(0.6f, 0.7f, 0.85f), UiNavy);
            playAgain.gameObject.AddComponent<ReloadSceneOnClick>().Bind(playAgain);

            return overlayRt.gameObject;
        }

        private static Text BuildStarChip(RectTransform parent,
            Vector2 anchorMin, Vector2 anchorMax, float bulbScale)
        {
            var shadow = MakePanel(parent, "StarChip_Shadow", anchorMin, anchorMax,
                new Color(0f, 0f, 0f, 0.15f));
            shadow.offsetMin = new Vector2(0f, -8f);
            shadow.offsetMax = new Vector2(0f, -2f);
            var sImg = shadow.GetComponent<Image>();
            var knob = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
            if (knob != null) { sImg.sprite = knob; sImg.preserveAspect = true; }

            var chip = MakePanel(parent, "StarChip", anchorMin, anchorMax, UiYellow);
            var cImg = chip.GetComponent<Image>();
            if (knob != null) { cImg.sprite = knob; cImg.preserveAspect = true; }

            var starTxt = MakeLabel(chip, "Star", "★",
                new Vector2(0f, 0f), new Vector2(1f, 1f),
                Mathf.RoundToInt(78f * bulbScale), TextAnchor.MiddleCenter, UiYellowShadow);
            starTxt.fontStyle = FontStyle.Bold;
            starTxt.raycastTarget = false;
            return starTxt;
        }

        // ── Failure overlay (Stitch-style "Oops" card) ─────────────────────

        private static GameObject BuildFailureOverlay(Transform parent,
            out Text titleText, out Text messageText, out Button retryBtn)
        {
            var overlayRt = MakePanel(parent, "FailureOverlay",
                new Vector2(0f, 0f), new Vector2(1f, 1f), UiOverlayDim);
            overlayRt.gameObject.SetActive(false);

            var cardShadow = MakePanel(overlayRt, "Card_Shadow",
                new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.82f),
                new Color(0f, 0f, 0f, 0.22f));
            cardShadow.offsetMin = new Vector2(0f, -16f);
            cardShadow.offsetMax = new Vector2(0f, -4f);
            ApplyRounded(cardShadow.GetComponent<Image>(), 0.7f);

            var card = MakePanel(overlayRt, "Card",
                new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.82f), UiCardWhite);
            ApplyRounded(card.GetComponent<Image>(), 0.7f);

            // Red header pill at top.
            var headerPillShadow = MakePanel(card, "HeaderPill_Shadow",
                new Vector2(0.20f, 0.84f), new Vector2(0.80f, 0.94f), UiRedShadow);
            headerPillShadow.offsetMin = new Vector2(0f, -10f);
            headerPillShadow.offsetMax = new Vector2(0f, -2f);
            ApplyRounded(headerPillShadow.GetComponent<Image>(), 0.25f);

            var headerPill = MakePanel(card, "HeaderPill",
                new Vector2(0.20f, 0.84f), new Vector2(0.80f, 0.94f), UiRed);
            ApplyRounded(headerPill.GetComponent<Image>(), 0.25f);
            var headerTxt = MakeLabel(headerPill, "Label", "OOPS!",
                new Vector2(0f, 0f), new Vector2(1f, 1f), 48, TextAnchor.MiddleCenter, Color.white);
            headerTxt.fontStyle = FontStyle.Bold;
            headerTxt.raycastTarget = false;

            titleText = MakeLabel(card, "Title", "Accident!",
                new Vector2(0.05f, 0.66f), new Vector2(0.95f, 0.82f),
                72, TextAnchor.MiddleCenter, UiRed);
            titleText.fontStyle = FontStyle.Bold;

            messageText = MakeLabel(card, "Message",
                "You were on the zebra when the cars had a GREEN light.\n" +
                "Wait for RED, then cross safely.",
                new Vector2(0.06f, 0.34f), new Vector2(0.94f, 0.66f),
                34, TextAnchor.MiddleCenter, UiNavy);

            retryBtn = MakeStitchPillButton(card, "RetryButton", "RETRY",
                new Vector2(0.16f, 0.10f), new Vector2(0.84f, 0.26f),
                UiYellow, UiYellowShadow, UiNavy);

            return overlayRt.gameObject;
        }

        // ── Rounded-corner helper ───────────────────────────────────────────
        // Pass `roundness`: smaller value = bigger corner radius.
        //   0.20-0.40 → pill-shaped (very rounded)
        //   0.60-1.00 → card (subtly rounded)
        //   2.00+     → sharp corners
        private static void ApplyRounded(Image img, float roundness)
        {
            var rounded = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            if (rounded == null) return;
            img.sprite = rounded;
            img.type = Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = Mathf.Max(0.05f, roundness);
        }

        // ── UI builders ──────────────────────────────────────────────────────

        private static RectTransform MakePanel(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            return rt;
        }

        private static RectTransform MakePanel(RectTransform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            return MakePanel((Transform)parent, name, anchorMin, anchorMax, color);
        }

        private static Image MakeImage(RectTransform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            return img;
        }

        private static Text MakeLabel(Transform parent, string name, string text,
            Vector2 anchorMin, Vector2 anchorMax, int fontSize, TextAnchor align, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var t = go.AddComponent<Text>();
            t.text = text;
            t.alignment = align;
            t.color = color;
            t.fontSize = fontSize;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Truncate;
            return t;
        }

        // ── Build settings ───────────────────────────────────────────────────

        private static void AddSceneToBuildSettings(string path)
        {
            var scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].path == path) return;
            }
            var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes)
            {
                new EditorBuildSettingsScene(path, true)
            };
            EditorBuildSettings.scenes = list.ToArray();
        }

        // ── Primitive helpers ────────────────────────────────────────────────

        private static Transform CreateCube(string name, Transform parent, Vector3 localPos,
            Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            ApplyURPColor(go.GetComponent<MeshRenderer>(), color);
            return go.transform;
        }

        // Strip the collider off a primitive — used for purely decorative
        // meshes that should never participate in physics (thin road
        // markings, signs, door knobs, etc). Reduces physics overhead and
        // eliminates clipping edge cases where the kid's CharacterController
        // could snag on a 3-cm decal.
        private static void StripCollider(Transform t)
        {
            if (t == null) return;
            var col = t.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
        }

        private static void StripCollidersRecursive(Transform t)
        {
            if (t == null) return;
            foreach (var c in t.GetComponentsInChildren<Collider>(true))
            {
                // Keep triggers (coins, crosswalk zones, goals).
                if (c.isTrigger) continue;
                Object.DestroyImmediate(c);
            }
        }

        private static Transform CreatePlane(string name, Transform parent, Vector3 localPos,
            Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            ApplyURPColor(go.GetComponent<MeshRenderer>(), color);
            return go.transform;
        }

        private static Transform CreateSphere(string name, Transform parent, Vector3 localPos,
            Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            ApplyURPColor(go.GetComponent<MeshRenderer>(), color);
            return go.transform;
        }

        private static Transform CreateCylinder(string name, Transform parent, Vector3 localPos,
            Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            ApplyURPColor(go.GetComponent<MeshRenderer>(), color);
            return go.transform;
        }

        private static void ApplyURPColor(MeshRenderer mr, Color color)
        {
            // Slight default sheen so everything reads as a real material under
            // post-processing instead of looking like flat clay.
            ApplyPBR(mr, color, metallic: 0.0f, smoothness: 0.25f);
        }

        private static void ApplyPBR(MeshRenderer mr, Color color, float metallic, float smoothness)
        {
            if (mr == null) return;
            mr.sharedMaterial = BuildPBRMaterial(color, metallic, smoothness);
        }

        private static Material BuildPBRMaterial(Color color, float metallic, float smoothness)
        {
            var shader = Shader.Find("Standard")
                       ?? Shader.Find("Universal Render Pipeline/Lit")
                       ?? Shader.Find("Diffuse");
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   Mathf.Clamp01(metallic));
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", Mathf.Clamp01(smoothness));
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", Mathf.Clamp01(smoothness));
            return mat;
        }

        private static void ApplyEmissive(MeshRenderer mr, Color color, float intensity)
        {
            if (mr == null) return;
            var shader = Shader.Find("Standard")
                       ?? Shader.Find("Universal Render Pipeline/Lit")
                       ?? Shader.Find("Diffuse");
            var mat = new Material(shader);
            // Black base so the emission "is" the look.
            if (mat.HasProperty("_Color"))     mat.SetColor("_Color",     Color.black);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", Color.black);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * intensity);
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            mr.sharedMaterial = mat;
        }
    }
}
