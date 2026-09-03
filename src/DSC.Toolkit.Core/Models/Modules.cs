namespace DSC.Toolkit.Core.Models;

public sealed record ToolDefinition(string Id, string Name, string Description, int SourcePage, string[] Fields);

public sealed record ModuleDefinition(
    string Id,
    string Planet,
    string Title,
    string Description,
    string AccentHex,
    IReadOnlyList<ToolDefinition> Tools);

public static class ModuleCatalog
{
    public static IReadOnlyList<ModuleDefinition> All { get; } =
    [
        new("luminaris", "Luminaris", "Campaign & Sessions", "Shape the campaign promise, sessions, arcs, and timeline.", "#E6CF85",
        [
            Tool("campaign-foundation", "Campaign Foundation", 163, "Campaign Promise", "Starting World and Community", "Initial Scale", "Central Activity", "Recurring Cost", "Known Cosmic Truths", "Important Common Beliefs", "First Discoverable Answer", "Lines and Boundaries", "Home Worth Protecting"),
            Tool("session-zero", "Session Zero Record", 163, "Politics, Exploration, Horror, and Combat Balance", "Corruption and Transformation", "Coma, Possession, and Loss of Agency", "Character Death and Legacy Play", "Permanent Sacrifice", "Player Secrets", "Travel Detail", "Player Character Shard-Bearing", "Safety and Revision Process"),
            Tool("campaign-arc", "Campaign Arc Sheet", 171, "Disturbance", "Connection", "Commitment", "Reversal", "Decision", "Aftermath"),
            Tool("campaign-timeline", "Campaign Timeline", 172, "Date or Session", "World", "Public Event", "Hidden Action", "Heroic Choice", "Delayed Consequence")
        ]),
        new("umbraria", "Umbraria", "Factions & Conspiracies", "Track political masks, dependencies, reputation, and hidden beneficiaries.", "#A980D6",
        [
            Tool("faction-profile", "Faction Profile", 165, "Name and World", "Public Mandate", "Private Goal", "Constituency", "Essential Resource", "Current Pressure", "Red Line", "Acceptable Compromise", "Rationalized Atrocity", "Internal Division", "Relationship to the Heroes"),
            Tool("relationship-map", "Faction Relationship Map", 165, "Faction A", "Faction B", "Public Relationship", "Private Dependence", "Breaking Point"),
            Tool("reputation", "Reputation Record", 165, "World or Faction", "Known Deed", "Public Story", "Interpretation", "Current Standing"),
            Tool("conspiracy", "Conspiracy Web", 165, "Layer", "Actor", "What They Know", "What They Want", "Evidence Left Behind")
        ]),
        new("eternum", "Eternum", "Dynamic Forces & Clocks", "Let opposition learn, act, and reveal warning signs under GM control.", "#B84A4A",
        [
            Tool("force-profile", "Dynamic Force Profile", 164, "Force Name", "Type and Scale", "Desired Future", "Public Goal", "Hidden Need", "Favored Method", "Assets", "Constraint", "Fear", "Recurring Tell", "Current Target"),
            Tool("force-turn", "Force Turn", 164, "What changed since the last turn?", "What does the force now know?", "What does it incorrectly believe?", "Which asset does it use?", "What action does it take?", "What sign appears first?", "Which clock advances?", "What can the heroes interrupt?"),
            Tool("adaptation", "Adaptation Ledger", 164, "Defeat or Interference", "Lesson Learned", "Method Abandoned", "New Method", "False Conclusion"),
            Tool("front", "Front Tracker", 171, "Front", "Desired Future", "Next Step", "Warning Sign", "Clock", "Connection")
        ]),
        new("solara", "Solara", "Worlds & Adventures", "Build pressures, people, settlements, mysteries, and changed-world consequences.", "#D9903D",
        [
            Tool("world", "World Situation Sheet", 169, "World", "Immediate Pressure", "Ordinary People's Need", "Powerful Gatekeeper", "Environmental Identity", "Active Factions", "Hidden Interworld Link", "Moral Division", "Current Lock State", "Planetary Phenomenon", "Return Hook"),
            Tool("settlement", "City and Settlement Sheet", 169, "Name, World, and Scale", "First Impression", "Essential Work", "Who Governs", "Who Actually Decides", "Who Is Excluded", "Important District or Landmark", "Local Custom", "Scarcity", "Current Rumor", "Threat", "Person Worth Remembering"),
            Tool("npc", "Major NPC Record", 169, "Name, Race, and Pronouns", "Role and Affiliation", "Immediate Want", "Long-Term Hope", "Fear", "Leverage", "Boundary", "Mannerism or Tell", "Connection to a Hero", "Secret", "How They Change"),
            Tool("adventure", "Adventure Situation", 170, "Disturbance", "Objective", "Stakes", "Active Forces", "Escalation", "Three Reliable Clues", "Two Defensible Priorities", "Site or Route", "Reward Beyond Treasure", "Changed World Afterward"),
            Tool("mystery", "Mystery Record", 170, "Question", "Clue One", "Clue Two", "Clue Three", "False Interpretation"),
            Tool("encounter", "Antagonist Encounter Sheet", 170, "Scene Purpose", "Antagonist Objective", "Heroic Leverage", "Bargain Offered", "Contradiction Revealed", "Exit Conditions", "Public Witness", "Clock Change", "Consequence of Violence")
        ]),
        new("aetheria", "Aetheria", "Travel & Expeditions", "Manage gates, vessels, unequal access, routes, hazards, and arrivals.", "#69BDEB",
        [
            Tool("journey", "Interworld Journey Sheet", 167, "Origin and Destination", "Method: Gate or Vessel", "Reason for Travel", "Route Authority", "Passengers and Unequal Access", "Essential Supply", "Navigation Sign", "Expected Duration", "Hazard", "Social Pressure", "Discovery", "Arrival Consequence"),
            Tool("vessel", "Celestial Vessel Record", 168, "Vessel Name and Type", "Captain and Allegiance", "Crew Need", "Route Strength", "Current Damage", "Unusual Capability", "Cargo", "Passenger Conflict", "Secret", "Debt or Obligation"),
            Tool("gate", "Planetary Gate Record", 168, "Gate and Location", "Controlling Power", "Legal Access", "Actual Access", "Toll or Obligation", "Merchant Privilege", "Common-Folk Burden", "Failure Sign", "Smuggling Route", "Political Consequence"),
            Tool("expedition", "Expedition Sheet", 168, "Objective", "Patron and Hidden Interest", "Route", "Guide", "Supply Limit", "Environmental Pressure", "Cultural Boundary", "Rival Expedition", "Discovery", "Extraction Problem", "Lasting Impact")
        ]),
        new("harmonia", "Harmonia", "Rituals & Relics", "Design consequential rituals and relics with costs, claims, and misuse.", "#6FC78A",
        [
            Tool("ritual", "Ritual Design Sheet", 166, "Ritual Name and Scale", "Intended Change", "Key", "Focus", "Offering", "Anchor", "Witness", "Required Alignment", "Roles", "Preparation Signs", "Failure Consequence", "Success Cost", "Forbidden Principle Used"),
            Tool("ritual-phases", "Ritual Phases", 167, "Preparation", "Opening", "Contest", "Binding", "Aftermath"),
            Tool("relic", "Relic Record", 167, "Relic Name", "Origin and World", "Category", "Appearance", "Dormant Function", "Awakened Function", "Awakening Condition", "Cost", "Faction Claim", "Evidence of Misuse", "Connection to a Guardian, Shard, or Route")
        ]),
        new("celestia", "Celestia", "Shards, Locks & Endgames", "Track hosts, corruption stages, Sword memory, Locks, sacrifice, and aftermath.", "#DDE7EE",
        [
            Tool("shard-host", "Shard and Host Record", 165, "Sword and Opposing World", "Current Host", "Host's Race and Background", "Bound at Birth or Transferred", "Original Desire", "Virtue Exploited", "Present Temptation", "Visible Cost", "Host's Knowledge", "Protectors and Hunters", "Transfer History", "Shard-Seeker Compass Response"),
            Tool("umbrarian", "Umbrarian Progress", 166, "Whisper", "Gift", "Exception", "System", "Claim"),
            Tool("sword-memory", "Sword Memory Ledger", 166, "Host and Era", "Virtue Exploited", "Most Effective Lie", "Failed Temptation", "Institution Studied", "Heroic Tactic Observed", "Future Adaptation", "Estimated Reappearance: unknown, decades to eons"),
            Tool("seven-locks", "Seven Locks Record", 171, "World", "Guardian", "Sword", "Lock State", "Phenomena", "Mortal Successor"),
            Tool("endgame", "Endgame Session Sheet", 171, "Which Lock is under pressure?", "What sign reaches ordinary people?", "Which antagonist acts during delay?", "What fact can change the plan?", "Who is being asked to sacrifice?", "Is the cost understood and freely chosen?", "What alternative remains untested?", "Which relationship needs a final scene?", "What changes even after success?")
        ])
    ];

    private static ToolDefinition Tool(string id, string name, int page, params string[] fields) =>
        new(id, name, $"Operational worksheet from DMG Chapter 12, page {page}.", page, fields);
}
