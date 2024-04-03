# Telepresence

Telepresence is a tool for bridging your local machine to a kubernetes cluster.

Just because your cluster happens to be on the local machine, does not remove the need for this. For instance, environment variables in the cluster are not available outside the cluster. Likewise, volumes, network routes, and other pods may not be accessible without being within the cluster environment.

Install telepresence to your host machine.

Telepresence will be connected to your Docker-Desktop cluster, as it is the current kubernetes context. However, you need to install the telepresence agent into your cluster for telepresence to properly intercept applications.

from a powershell terminal, run the following telepresence command:

```
telepresence helm install
```

from there, you should be able to launch telepresence:

```
telepresence connect
```

You can then list discovered services using:

```
telepresence list
```

You can check your connection to the kubernetes cluster is active by looking up a service in a browser.

```
http://<serviceName>.default.svc.cluster.local/
```

eg:

```
http://http://chart-gitea-http.default.svc.cluster.local/
```

It is also possible to redirect traffic destined to a cluster service to a local debugging session outside of the cluster.

https://www.telepresence.io/docs/latest/quick-start/

if tilt gives you an error about conflicting subnets, you can re-install tilt into your cluster with conflicts allowed like so:
```
>telepresence helm install
Traffic Manager installed successfully

> telepresence connect
Launching Telepresence User Daemon
telepresence connect: error: connector.Connect: subnet 172.30.145.0/24 overlaps with existing route "172.30.144.0/20 via 172.30.144.1 dev vEthernet (WSL (Hyper-V firewall)), gw 0.0.0.0". Please see https://www.getambassador.io/docs/telepresence/latest/reference/vpn for more information

>telepresence helm uninstall
Traffic Manager uninstalled successfully

>telepresence telepresence helm install  --set client.routing.allowConflictingSubnets='{172.30.144.0/20}'

>telepresence connect
Connected to context microk8s, namespace default (https://172.30.145.142:16443)
```