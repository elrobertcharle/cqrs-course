docker.build.producer:
	docker build -t post.cmd.api.producer:1.0 -f Dockerfile.Post.Cmd.Api.Producer .
	docker compose -f ./docker/Post.Cmd.Api.Producer/docker-compose.yml up -d --force-recreate