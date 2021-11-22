@Library("JenkinsPipelineLibrary") _

pipeline{
  agent {
    label "slaveIRIS"
  }

  options{
    buildDiscarder(logRotator(numToKeepStr: '5'))
    disableConcurrentBuilds()
    gitLabConnection('Gitlab')
  }

  triggers{
    gitlab(triggerOnPush: true, triggerOnMergeRequest: true, branchFilterType: 'All')
  }

  stages{
    stage('Build'){
      steps {
        script{
          pipelineLib.beginSonarQubeForMsBuild("HRP", "Nexus / HRP", "/d:sonar.cs.opencover.reportsPaths=%WORKSPACE%\\coverage.xml /d:sonar.cs.xunit.reportsPaths=%WORKSPACE%\\UnitTest.xml")

          pipelineLib.restoreNuget()

          pipelineLib.msBuild("HRP.sln")
        }
      }
    }
	
	stage("C# Unit Test") {
	  steps{
        script{
          pipelineLib.xUnitForNetFramework()
        }
      }
    }

    stage("SonarQube Analysis"){
      steps{
        script{
          pipelineLib.endSonarQubeForMsBuild()
        }
      }

      post{
        always{
          script{
            if(manager.logContains(".*Quality gate status.*")){
              pipelineLib.generateSonarQubeReport("HRP")
            }
          }
        }
      }
    }
  }

  post{
	always{
      archiveArtifacts "*.xml,*.email"
      step([$class: 'ACIPluginPublisher', name: '*.xml', shownOnProjectPage: false])
      cobertura coberturaReportFile: "cobertura.xml", onlyStable: false
      xunit thresholds: [failed(failureNewThreshold: '1', failureThreshold: '1')], tools: [xUnitDotNet(deleteOutputFiles: true, failIfNotNew: true, pattern: 'UnitTest.xml', skipNoTestFiles: false, stopProcessingIfError: true)]
    }
    unsuccessful{
      emailext body: '$DEFAULT_CONTENT', subject: '$DEFAULT_SUBJECT', to: 'quinn.vo@starixvn.com,tim.trinh@starixvn.com'
    }
  }
}