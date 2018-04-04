def config;

node {
    stage('checkout') {
        // git 'https://github.com/wuqi618/PipelineSample.git'
        checkout scm
    }
    
    // stage('build') {
    //    dir('src/WebApiSample/') {
    //        sh 'dotnet restore -r ubuntu.16.04-x64'
    //        sh 'dotnet build -c Release -r ubuntu.16.04-x64'
    //    }
    // }
    
    // stage('test') {
    //     dir('src/WebApiSample/Tests/') {
    //         sh 'dotnet xunit -c Release -xml TestResult/TestResult.xml'
    //     }
    // }
    
    stage('publish') {
        def path = "${pwd()}/CloudFormation/ecs-WebApiSample.config";
        config = readJSON file: path;

        dir('src/WebApiSample/WebApiSample/') {
            publish(config);
        }
    }

    stage('deploy') {
        dir('CloudFormation/') {
            def path = pwd();
            def buildNumber = "${BUILD_NUMBER}";
            //deploy(path, "ecs-WebApiSample.yml", "ecs-WebApiSample.config", buildNumber);
            //def dir = sh returnStdout: true, script: 'echo $PWD'
            //dir = dir.trim()
            //def image = "531585151505.dkr.ecr.ap-southeast-2.amazonaws.com/webapisample:${BUILD_NUMBER}"
            //def cluster = "pipeline-ecs-cluster"
            //def vpc = "vpc-9c42f4fb"
            //def subnets = "subnet-aa7e1de3\\\\,subnet-98c7b1ff"
            //sh "aws cloudformation create-stack --stack-name webapisample --template-body file://${dir}/ecs-WebApiSample.yml --parameters ParameterKey=Image,ParameterValue=${image} ParameterKey=ECSCluster,ParameterValue=${cluster} ParameterKey=VPC,ParameterValue=${vpc} ParameterKey=Subnets,ParameterValue=${subnets} --region ap-southeast-2"
        }
    }
    
    // stage('zip & s3') {
    //     dir('src/WebApiSample/WebApiSample/') {
    //         sh 'rm -f publish.zip'
    //         sh 'zip -r publish.zip Publish'
    //         sh "aws s3 cp publish.zip s3://webapisample-publish/publish-${BUILD_NUMBER}.zip"
    //     }
    // }
    
    // stage('archive') {
    //     dir('src/WebApiSample/Tests/') {
    //         step([$class: 'XUnitPublisher', testTimeMargin: '3000', thresholdMode: 1, thresholds: [[$class: 'FailedThreshold', failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0'], [$class: 'SkippedThreshold', failureNewThreshold: '', failureThreshold: '', unstableNewThreshold: '', unstableThreshold: '']], tools: [[$class: 'XUnitDotNetTestType', deleteOutputFiles: true, failIfNotNew: true, pattern: 'TestResult/*.xml', skipNoTestFiles: false, stopProcessingIfError: true]]])
    //     }
        
    //     dir('src/WebApiSample/WebApiSample/Publish/') {
    //         archiveArtifacts '**'
    //     }
    // }
}

def publish(config) {
    // sh 'rm -rf Publish'
    // sh 'dotnet publish WebApiSample.csproj -c Release -r ubuntu.16.04-x64 -o Publish'
    
    def image = "${config.ecr}:${BUILD_NUMBER}";
    def output = sh returnStdout: true, script: "aws ecr get-login --region ${config.region}"
    output = output.replaceFirst(" -e none ", " ")
    echo output
    sh "${output}"
    sh "docker build -t webapisample -f Dockerfile.ci ."
    sh "docker tag webapisample:latest ${image}"
    sh "docker push ${image}"
}

def deploy(config, path, template, buildNumber) {
    def config = parseConfig(config);

    if(stackExists(config)) {
        echo "updateStack";
        //updateStack(config, path, template);
    } else {
        echo "createStack"
        //createStack(config, path, template);
    }
}

def parseConfig(config) {
    config.ecr = "${config.ecr}:${BUILD_NUMBER}";
    config.subnets = config.subnets.join('\\\\,');
    config.template = "${WORKSPACE}/${config.template}"
    return config;
}

def Boolean stackExists(config) {
    def exitValue = sh returnStatus: true, script: 'aws cloudformation describe-stacks --stack-name ${config.stackName} --region ${config.region}';
    return exitValue == 0;
}

def createStack(config) {
    sh "aws cloudformation create-stack --stack-name ${config.stackName} --template-body file://${config.template} --parameters ParameterKey=Image,ParameterValue=${config.ecr} ParameterKey=ECSCluster,ParameterValue=${config.cluster} ParameterKey=VPC,ParameterValue=${config.vpc} ParameterKey=Subnets,ParameterValue=${config.subnets} --region ${config.region}";
    sh "aws cloudformation wait stack-create-complete  --stack-name ${config.stackName} --region ${config.region}"
}

def updateStack(config) {
    sh "aws cloudformation update-stack --stack-name ${config.stackName} --template-body file://${config.template} --parameters ParameterKey=Image,ParameterValue=${config.ecr} ParameterKey=ECSCluster,ParameterValue=${config.cluster} ParameterKey=VPC,ParameterValue=${config.vpc} ParameterKey=Subnets,ParameterValue=${config.subnets} --region ${config.region}";
    sh "aws cloudformation wait stack-update-complete  --stack-name ${config.stackName} --region ${config.region}"
}