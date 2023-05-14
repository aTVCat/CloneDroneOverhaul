// I took code from here: https://github.com/Facepunch/Facepunch.Steamworks/blob/master/Generator/steam_sdk/isteamgameserver.h
#define USE_GS_AUTH_API

using CDOverhaul;
using Steamworks;
using UnityEngine;

public class GameServerTest : MonoBehaviour
{
    private static string ServerVersion;
    private const ushort SPACEWAR_AUTHENTICATION_PORT = 8766;
    private const ushort SPACEWAR_SERVER_PORT = 27015;
    private const ushort SPACEWAR_MASTER_SERVER_UPDATER_PORT = 27016;

    protected Callback<SteamServersConnected_t> m_CallbackSteamServersConnected;
    protected Callback<SteamServerConnectFailure_t> m_CallbackSteamServersConnectFailure;
    protected Callback<SteamServersDisconnected_t> m_CallbackSteamServersDisconnected;
    protected Callback<GSPolicyResponse_t> m_CallbackPolicyResponse;
    protected Callback<ValidateAuthTicketResponse_t> m_CallbackGSAuthTicketResponse;
    protected Callback<P2PSessionRequest_t> m_CallbackP2PSessionRequest;
    protected Callback<P2PSessionConnectFail_t> m_CallbackP2PSessionConnectFail;

    public string m_strServerName = "Overhaul MOD";
    public string m_strMapName = "Arena";
    public int m_nMaxPlayers = 15;
    private bool m_bInitialized;
    private bool m_bConnectedToSteam;

    private CSteamID m_ServerID;

    private void OnEnable()
    {
        ServerVersion = OverhaulVersion.ModVersion.ToString();

        m_CallbackSteamServersConnected = Callback<SteamServersConnected_t>.CreateGameServer(OnSteamServersConnected);

        m_CallbackSteamServersConnectFailure = Callback<SteamServerConnectFailure_t>.CreateGameServer(OnSteamServersConnectFailure);
        m_CallbackSteamServersDisconnected = Callback<SteamServersDisconnected_t>.CreateGameServer(OnSteamServersDisconnected);
        m_CallbackPolicyResponse = Callback<GSPolicyResponse_t>.CreateGameServer(OnPolicyResponse);

        m_CallbackGSAuthTicketResponse = Callback<ValidateAuthTicketResponse_t>.CreateGameServer(OnValidateAuthTicketResponse);
        m_CallbackP2PSessionRequest = Callback<P2PSessionRequest_t>.CreateGameServer(OnP2PSessionRequest);
        m_CallbackP2PSessionConnectFail = Callback<P2PSessionConnectFail_t>.CreateGameServer(OnP2PSessionConnectFail);

        m_bInitialized = false;
        m_bConnectedToSteam = false;

        EServerMode eMode = EServerMode.eServerModeAuthenticationAndSecure;

        m_bInitialized = GameServer.Init(0, SPACEWAR_AUTHENTICATION_PORT, SPACEWAR_SERVER_PORT, SPACEWAR_MASTER_SERVER_UPDATER_PORT, eMode, ServerVersion);
        if (!m_bInitialized)
        {
            Debug.Log("SteamGameServer_Init call failed");
            return;
        }
        print("Initialized");
        SteamGameServer.SetModDir("Clone Drone in the Danger Zone");
        SteamGameServer.SetProduct("Clone Drone in the Danger Zone");
        SteamGameServer.SetGameDescription("Clone Drone in the Danger Zone");
        SteamGameServer.LogOnAnonymous();
        SteamGameServer.EnableHeartbeats(true);

        Debug.Log("Started.");
    }

    private void OnDisable()
    {
        if (!m_bInitialized)
        {
            return;
        }

        SteamGameServer.EnableHeartbeats(false);

        m_CallbackSteamServersConnected.Dispose();
        SteamGameServer.LogOff();

        GameServer.Shutdown();
        m_bInitialized = false;

        Debug.Log("Shutdown.");
    }

    private void Update()
    {
        if (!m_bInitialized)
        {
            return;
        }

        GameServer.RunCallbacks();

        if (m_bConnectedToSteam)
        {
            SendUpdatedServerDetailsToSteam();
        }
    }

    private void OnSteamServersConnected(SteamServersConnected_t pLogonSuccess)
    {
        Debug.Log("SpaceWarServer connected to Steam successfully");
        m_bConnectedToSteam = true;

        SendUpdatedServerDetailsToSteam();
    }

    private void OnSteamServersConnectFailure(SteamServerConnectFailure_t pConnectFailure)
    {
        m_bConnectedToSteam = false;
        Debug.Log("SpaceWarServer failed to connect to Steam");
    }

    private void OnSteamServersDisconnected(SteamServersDisconnected_t pLoggedOff)
    {
        m_bConnectedToSteam = false;
        Debug.Log("SpaceWarServer got logged out of Steam");
    }

    private void OnPolicyResponse(GSPolicyResponse_t pPolicyResponse)
    {

        if (SteamGameServer.BSecure())
        {
            Debug.Log("SpaceWarServer is VAC Secure!");
        }
        else
        {
            Debug.Log("SpaceWarServer is not VAC Secure!");
        }

        m_ServerID = SteamGameServer.GetSteamID();
        Debug.Log("Game server SteamID: " + m_ServerID.ToString());
    }

    private void OnValidateAuthTicketResponse(ValidateAuthTicketResponse_t pResponse)
    {
        Debug.Log("OnValidateAuthTicketResponse Called steamID: " + pResponse.m_SteamID);
    }

    private void OnP2PSessionRequest(P2PSessionRequest_t pCallback)
    {
        Debug.Log("OnP2PSesssionRequest Called steamIDRemote: " + pCallback.m_steamIDRemote);
        SteamGameServerNetworking.AcceptP2PSessionWithUser(pCallback.m_steamIDRemote);
    }

    private void OnP2PSessionConnectFail(P2PSessionConnectFail_t pCallback)
    {
        Debug.Log("OnP2PSessionConnectFail Called steamIDRemote: " + pCallback.m_steamIDRemote); // Riley
    }

    private void SendUpdatedServerDetailsToSteam()
    {
        SteamGameServer.SetMaxPlayerCount(m_nMaxPlayers);
        SteamGameServer.SetPasswordProtected(false);
        SteamGameServer.SetServerName(m_strServerName);
        SteamGameServer.SetBotPlayerCount(0);
        SteamGameServer.SetMapName(m_strMapName);
    }
}