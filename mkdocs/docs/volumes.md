# Persistent Volumes

For development, we do our best to avoid needing persistent volumes. When the servers are shut down and launched again, it should be a full system reset.

Instead, we try and rely on packaging our state into text files tracked through git, typically as kubernetes yaml files, helm configuration, environment variables, and the occasional json file.

Another option for more complex scenarios is to add scripts to our installation job.

The exception is if the environment interacts with external services in a way that must maintain state, particularly if that state is sensitive.

In the case of SSL certs, for instance, these are provisioned to traefik through namecheap and letsencrypt. Letsencrypt has strict rate limits, and provisioning new SSL certificates on each restart would quickly reach those limits, locking out any new certificate creation until the rate limits have expired. On top of that, committing live SSL certificates to git is not a great security practice. For this reason, we have a persistent volume that caches the certs and it will keep those between server shutdowns.

Local volumes:

I found a solution to this recently though it has some drawbacks.

So I'm going to make a few assumptions:

- you're probably using docker-desktop
- you only have a single k8s node

you can create a kubernetes persistent volume that is local filesystem storage. That is to say, it lives on the disk of a single node. In multi node environments, this has limited uses because pod scheduling can happen on any node, but for tilt it works just fine.

Now when you do this, the trick I bumped into (and many others, after looking it up) is that if you are on windows, your 'node' is not your windows machine, but rather a linux VM that docker-desktop uses to run containers in. So at first, you can easily share a file with all pods, but the local machine isn't there by default:

```
apiVersion: v1
kind: PersistentVolume
metadata:
  name: traefik-pv
spec:
  capacity:
    storage: 128Mi
  volumeMode: Filesystem
  accessModes:
  - ReadWriteOnce
  persistentVolumeReclaimPolicy: Retain
  storageClassName: local-storage
  hostPath:
    path: /mnt/data
    type: Directory
```

however, your docker-desktop VM does have access to the windows C:/ disk

you can use the path "/run/desktop/mnt/host/c" to access it.

```
apiVersion: v1
kind: PersistentVolume
metadata:
  name: traefik-pv
spec:
  capacity:
    storage: 128Mi
  volumeMode: Filesystem
  accessModes:
  - ReadWriteOnce
  persistentVolumeReclaimPolicy: Retain
  storageClassName: local-storage
  hostPath:
    path: /run/desktop/mnt/host/c/dev/data/traefik
    type: Directory
```

however, you will quickly notice that the performance you get when moving data from your docker VM to the windows hard drive is pretty abysmal. Like, its fine if you need to store a few static files and update them now and then, but if you are running a lot of IO over this bridge you are going to notice extreme lag.
I tried stashing my database on disk like this, and I quickly had to revert.