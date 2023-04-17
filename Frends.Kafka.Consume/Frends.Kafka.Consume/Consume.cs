﻿using Frends.Kafka.Consume.Definitions;
using System.ComponentModel;
using System;
using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;
using System.Threading;
using System.Collections.Generic;

namespace Frends.Kafka.Consume;

/// <summary>
/// Kafka Task
/// </summary>
public class Kafka
{
    /// <summary>
    /// Kafka Consume operation.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.Kafka.Consume)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="options">Optional parameters.</param>
    /// <param name="socket">Socket parametes.</param>
    /// <param name="sasl">SASL parameters.</param>
    /// <param name="ssl">SSL parameters.</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this task.</param>
    /// <returns>Object { bool Success, List&lt;Message&gt; Messages }</returns>
    public static Result Consume([PropertyTab] Input input, [PropertyTab] Options options, [PropertyTab] Socket socket, [PropertyTab] Sasl sasl, [PropertyTab] Ssl ssl, CancellationToken cancellationToken)
    {
        if (options.MaxPollIntervalMs < options.SessionTimeoutMs)
            throw new Exception("Options.MaxPollIntervalMs must be >= Options.SessionTimeoutMs");

        var result = new List<Message>();
        var config = Configurations(input, options, socket, sasl, ssl);

        using var consumer = new ConsumerBuilder<Null, string>(config).Build();
        consumer.Subscribe(input.Topic);
        try
        {
            while (result.Count < input.MessageCount)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var cr = input.Timeout > 0 ? consumer.Consume(input.Timeout) : consumer.Consume(cancellationToken);

                if (cr != null)
                    result.Add(new Message()
                    {
                        Key = cr.Message.Key?.ToString(),
                        Value = cr.Message.Value,
                    });
                else
                    break;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Consume error: {ex}");
        }
        finally
        {
            consumer.Close();
            consumer.Dispose();
        }

        return new Result(true, result);
    }

    private static ConsumerConfig Configurations(Input input, Options options, Socket socket, Sasl sasl, Ssl ssl)
    {
        ConsumerConfig config = new()
        {
            Acks = GetAcks(options),
            ApiVersionRequest = options.ApiVersionRequest,
            ApiVersionFallbackMs = options.ApiVersionFallbackMs,
            ApiVersionRequestTimeoutMs = options.ApiVersionRequestTimeoutMs,
            AllowAutoCreateTopics = options.AllowAutoCreateTopics,
            AutoCommitIntervalMs = options.AutoCommitIntervalMs,
            AutoOffsetReset = GetAutoOffsetReset(options),
            BootstrapServers = input.Host,
            BrokerAddressFamily = GetBrokerAddressFamily(options),
            ConnectionsMaxIdleMs = options.ConnectionsMaxIdleMs,
            CheckCrcs = options.CheckCrcs,
            EnableAutoCommit = options.EnableAutoCommit,
            EnableAutoOffsetStore = options.EnableAutoOffsetStore,
            FetchErrorBackoffMs = options.FetchErrorBackoffMs,
            FetchMaxBytes = options.FetchMaxBytes,
            FetchMinBytes = options.FetchMinBytes,
            FetchWaitMaxMs = options.FetchWaitMaxMs,
            GroupId = string.IsNullOrWhiteSpace(options.GroupId) ? "string" : options.GroupId,
            GroupInstanceId = string.IsNullOrWhiteSpace(options.GroupInstanceId) ? "" : options.GroupInstanceId,
            HeartbeatIntervalMs = options.HeartbeatIntervalMs,
            IsolationLevel = GetIsolationLevel(options),
            MaxPollIntervalMs = options.MaxPollIntervalMs,
            MaxInFlight = options.MaxInFlight,
            MessageMaxBytes = options.MessageMaxBytes,
            QueuedMaxMessagesKbytes = options.QueuedMaxMessagesKbytes,
            QueuedMinMessages = options.QueuedMinMessages,
            ReconnectBackoffMaxMs = options.ReconnectBackoffMaxMs,
            ReconnectBackoffMs = options.ReconnectBackoffMs,
            SecurityProtocol = GetSecurityProtocol(input),
            SocketTimeoutMs = socket.SocketTimeoutMs,
            SocketConnectionSetupTimeoutMs = socket.SocketConnectionSetupTimeoutMs,
            SocketKeepaliveEnable = socket.SocketKeepaliveEnable,
            SocketMaxFails = socket.SocketMaxFails,
            SocketNagleDisable = socket.SocketNagleDisable,
            SocketReceiveBufferBytes = socket.SocketReceiveBufferBytes,
            SessionTimeoutMs = options.SessionTimeoutMs,
        };

        if (sasl.UseSasl)
        {
            config.SaslMechanism = GetSaslMechanism(sasl);
            config.SaslUsername = sasl.SaslUsername;
            config.SaslPassword = sasl.SaslPassword;
            config.SaslOauthbearerMethod = GetSaslOauthbearerMethod(sasl);
            config.SaslOauthbearerClientId = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerClientId) ? "" : sasl.SaslOauthbearerClientId;
            config.SaslOauthbearerClientSecret = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerClientSecret) ? "" : sasl.SaslOauthbearerClientSecret;
            config.SaslOauthbearerTokenEndpointUrl = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerTokenEndpointUrl) ? "" : sasl.SaslOauthbearerTokenEndpointUrl;
            config.SaslOauthbearerConfig = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerConfig) ? "" : sasl.SaslOauthbearerConfig;
            config.SaslOauthbearerExtensions = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerExtensions) ? "" : sasl.SaslOauthbearerExtensions;
            config.SaslOauthbearerScope = string.IsNullOrWhiteSpace(sasl.SaslOauthbearerScope) ? "" : sasl.SaslOauthbearerScope;
            if (!string.IsNullOrEmpty(config.SaslKerberosKeytab))
            {
                config.SaslKerberosKeytab = sasl.SaslKerberosKeytab;
                config.SaslKerberosMinTimeBeforeRelogin = sasl.SaslKerberosMinTimeBeforeRelogin;
            }
            config.SaslKerberosPrincipal = sasl.SaslKerberosPrincipal;
            config.SaslKerberosServiceName = sasl.SaslKerberosServiceName;
        }

        if (ssl.UseSsl)
        {
            config.SslEndpointIdentificationAlgorithm = GetSslEndpointIdentificationAlgorithm(ssl);
            config.EnableSslCertificateVerification = ssl.EnableSslCertificateVerification;
            config.SslCertificateLocation = string.IsNullOrWhiteSpace(ssl.SslCertificateLocation) ? "" : ssl.SslCertificateLocation;
            config.SslCertificatePem = string.IsNullOrWhiteSpace(ssl.SslCertificatePem) ? "" : ssl.SslCertificatePem;
            if (!string.IsNullOrEmpty(ssl.SslCaCertificateStores))
                config.SslCaCertificateStores = ssl.SslCaCertificateStores;
            config.SslCaLocation = string.IsNullOrWhiteSpace(ssl.SslCaLocation) ? "" : ssl.SslCaLocation;
            config.SslCaPem = string.IsNullOrWhiteSpace(ssl.SslCaPem) ? "" : ssl.SslCaPem;
            config.SslKeyLocation = string.IsNullOrWhiteSpace(ssl.SslKeyLocation) ? "" : ssl.SslKeyLocation;
            config.SslKeyPassword = string.IsNullOrWhiteSpace(ssl.SslKeyPassword) ? "" : ssl.SslKeyPassword;
            config.SslKeyPem = string.IsNullOrWhiteSpace(ssl.SslKeyPem) ? "" : ssl.SslKeyPem;
            config.SslKeystoreLocation = string.IsNullOrWhiteSpace(ssl.SslKeystoreLocation) ? "" : ssl.SslKeystoreLocation;
            config.SslKeystorePassword = string.IsNullOrWhiteSpace(ssl.SslKeystorePassword) ? "" : ssl.SslKeystorePassword;
            config.SslEngineLocation = string.IsNullOrWhiteSpace(ssl.SslEngineLocation) ? "" : ssl.SslEngineLocation;
            config.SslCipherSuites = string.IsNullOrWhiteSpace(ssl.SslCipherSuites) ? "" : ssl.SslCipherSuites;
            config.SslCrlLocation = string.IsNullOrWhiteSpace(ssl.SslCrlLocation) ? "" : ssl.SslCrlLocation;
            config.SslCurvesList = string.IsNullOrWhiteSpace(ssl.SslCurvesList) ? "" : ssl.SslCurvesList;
            config.SslSigalgsList = string.IsNullOrWhiteSpace(ssl.SslSigalgsList) ? "" : ssl.SslSigalgsList;
        }

        return config;
    }

    [ExcludeFromCodeCoverage]
    private static SecurityProtocol GetSecurityProtocol(Input input)
    {
        return input.SecurityProtocol switch
        {
            SecurityProtocols.Plaintext => SecurityProtocol.Plaintext,
            SecurityProtocols.Ssl => SecurityProtocol.Ssl,
            SecurityProtocols.SaslPlaintext => SecurityProtocol.SaslPlaintext,
            SecurityProtocols.SaslSsl => SecurityProtocol.SaslSsl,
            _ => throw new Exception("GetSecurityProtocol error: Value not supported."),
        };
    }

    [ExcludeFromCodeCoverage]
    private static IsolationLevel GetIsolationLevel(Options options)
    {
        return options.IsolationLevel switch
        {
            IsolationLevels.ReadUncommitted => IsolationLevel.ReadUncommitted,
            IsolationLevels.ReadCommitted => IsolationLevel.ReadCommitted,
            _ => throw new Exception("GetIsolationLevel error: Value not supported."),
        };
    }

    [ExcludeFromCodeCoverage]
    private static BrokerAddressFamily GetBrokerAddressFamily(Options options)
    {
        return options.BrokerAddressFamily switch
        {
            BrokerAddressFamilys.Any => BrokerAddressFamily.Any,
            BrokerAddressFamilys.V4 => BrokerAddressFamily.V4,
            BrokerAddressFamilys.V6 => BrokerAddressFamily.V6,
            _ => throw new Exception("GetBrokerAddressFamily error: Value not supported."),
        };
    }

    [ExcludeFromCodeCoverage]
    private static AutoOffsetReset GetAutoOffsetReset(Options options)
    {
        return options.AutoOffsetReset switch
        {
            AutoOffsetResets.Latest => AutoOffsetReset.Latest,
            AutoOffsetResets.Earliest => AutoOffsetReset.Earliest,
            AutoOffsetResets.Error => AutoOffsetReset.Error,
            _ => throw new Exception("GetAutoOffsetReset error: Value not supported."),
        };
    }

    [ExcludeFromCodeCoverage]
    private static SslEndpointIdentificationAlgorithm GetSslEndpointIdentificationAlgorithm(Ssl ssl)
    {
        return ssl.SslEndpointIdentificationAlgorithm switch
        {
            SslEndpointIdentificationAlgorithms.None => SslEndpointIdentificationAlgorithm.None,
            SslEndpointIdentificationAlgorithms.Https => SslEndpointIdentificationAlgorithm.Https,
            _ => throw new Exception("SslEndpointIdentificationAlgorithm error: Value not supported."),
        };
    }

    [ExcludeFromCodeCoverage]
    private static Acks GetAcks(Options options)
    {
        return options.Acks switch
        {
            Ack.None => Acks.None,
            Ack.Leader => Acks.Leader,
            Ack.All => Acks.All,
            _ => throw new Exception("GetAcks error: Value not supported."),
        };
    }

    [ExcludeFromCodeCoverage]
    private static SaslOauthbearerMethod GetSaslOauthbearerMethod(Sasl sasl)
    {
        return sasl.SaslOauthbearerMethod switch
        {
            SaslOauthbearerMethods.Default => SaslOauthbearerMethod.Default,
            SaslOauthbearerMethods.Oidc => SaslOauthbearerMethod.Oidc,
            _ => throw new Exception("GetSaslOauthbearerMethod error: GetSaslOauthbearerMethod not supported."),
        };
    }

    [ExcludeFromCodeCoverage]
    private static SaslMechanism GetSaslMechanism(Sasl sasl)
    {
        return sasl.SaslMechanism switch
        {
            SaslMechanisms.Gssapi => SaslMechanism.Gssapi,
            SaslMechanisms.Plain => SaslMechanism.Plain,
            SaslMechanisms.ScramSha256 => SaslMechanism.ScramSha256,
            SaslMechanisms.ScramSha512 => SaslMechanism.ScramSha512,
            SaslMechanisms.OAuthBearer => SaslMechanism.OAuthBearer,
            _ => throw new Exception("GetSaslMechanism error: SaslMechanisms not supported."),
        };
    }
}